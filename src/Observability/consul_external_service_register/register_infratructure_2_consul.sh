#!/bin/sh

set -xeuo pipefail


curl --request PUT --data @/consul_external_service_register/prometheus.json consulserver:8500/v1/agent/service/register
curl --request PUT --data @/consul_external_service_register/dapr_placement.json consulserver:8500/v1/agent/service/register
curl --request PUT --data @/consul_external_service_register/dependencies.json consulserver:8500/v1/agent/service/register
curl --request PUT --data @/consul_external_service_register/grafana.json consulserver:8500/v1/agent/service/register
curl --request PUT --data @/consul_external_service_register/maildev.json consulserver:8500/v1/agent/service/register
curl --request PUT --data @/consul_external_service_register/mosquitto.json consulserver:8500/v1/agent/service/register
curl --request PUT --data @/consul_external_service_register/rabbitmq.json consulserver:8500/v1/agent/service/register
curl --request PUT --data @/consul_external_service_register/redis.json consulserver:8500/v1/agent/service/register
curl --request PUT --data @/consul_external_service_register/seq.json consulserver:8500/v1/agent/service/register
curl --request PUT --data @/consul_external_service_register/zipkin-mysql.json consulserver:8500/v1/agent/service/register
curl --request PUT --data @/consul_external_service_register/zipkin.json consulserver:8500/v1/agent/service/register