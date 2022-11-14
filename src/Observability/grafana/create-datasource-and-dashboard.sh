#!/bin/sh

set -xeuo pipefail

# Create Dapr data source
if ! curl --retry 5 --retry-connrefused --retry-delay 0 -sf http://grafana:3000/api/dashboards/name/Dapr; then
    curl -sf -X POST -H "Content-Type: application/json" \
         --data-binary '{"name":"Dapr","type":"prometheus","url":"http://prometheus:9090","access":"proxy","isDefault":true}' \
         http://grafana:3000/api/datasources
fi

# Create zipkin data source
if ! curl --retry 5 --retry-connrefused --retry-delay 0 -sf http://grafana:3000/api/dashboards/name/prom; then
    curl -sf -X POST -H "Content-Type: application/json" \
         --data-binary '{"name":"prom","type":"prometheus","url":"http://prometheus:9090","access":"proxy","isDefault":true}' \
         http://grafana:3000/api/datasources
fi

dashboard_id=1598
last_revision=$(curl -sf https://grafana.com/api/dashboards/${dashboard_id}/revisions | grep '"revision":' | sed 's/ *"revision": \([0-9]*\),/\1/' | sort -n | tail -1)

echo '{"dashboard": ' > data.json
curl -s https://grafana.com/api/dashboards/${dashboard_id}/revisions/${last_revision}/download >> data.json
echo ', "inputs": [{"name": "DS_PROMETHEUS", "pluginId": "prometheus", "type": "datasource", "value": "prom"}], "overwrite": false}' >> data.json

curl --retry-connrefused --retry 5 --retry-delay 0 -sf \
     -X POST -H "Content-Type: application/json" \
     --data-binary @data.json \
     http://grafana:3000/api/dashboards/import


# Import Dapr dashboards
# https://github.com/dapr/dapr/releases/download/v1.9.2/grafana-actor-dashboard.json
# https://github.com/dapr/dapr/releases/download/v1.9.2/grafana-sidecar-dashboard.json
# https://github.com/dapr/dapr/releases/download/v1.9.2/grafana-system-services-dashboard.json

echo '{"dashboard": ' > grafana-actor-dashboard.json
curl  -x http://172.16.4.20:3128 -L https://github.com/dapr/dapr/releases/download/v1.9.2/grafana-actor-dashboard.json >> grafana-actor-dashboard.json
echo ', "inputs": [{"name": "DS_PROMETHEUS", "pluginId": "prometheus", "type": "datasource", "value": "Dapr"}], "overwrite": false}' >> grafana-actor-dashboard.json

curl --retry-connrefused --retry 5 --retry-delay 0 -f \
     -X POST -H "Content-Type: application/json" \
     --data-binary @grafana-actor-dashboard.json \
     http://grafana:3000/api/dashboards/import


# curl -s -L https://github.com/dapr/dapr/releases/download/v1.9.2/grafana-sidecar-dashboard.json > grafana-sidecar-dashboard.json
# curl -s -L https://github.com/dapr/dapr/releases/download/v1.9.2/grafana-system-services-dashboard.json > grafana-system-services-dashboard.json
# curl --retry-connrefused --retry 5 --retry-delay 0 -sf \
#      -X POST -H "Content-Type: application/json" \
#      --data-binary @grafana-sidecar-dashboard.json \
#      http://grafana:3000/api/dashboards/import

# curl --retry-connrefused --retry 5 --retry-delay 0 -sf \
#      -X POST -H "Content-Type: application/json" \
#      --data-binary @grafana-system-services-dashboard.json \
#      http://grafana:3000/api/dashboards/import