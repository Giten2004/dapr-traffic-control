I have do some changes in the projects, So you can't run it in the selfhost mode at the moment.
because the I changed the path and the host name in dapr folder.

If you would like to run self host mode, you hava to use code from github: https://github.com/EdwinVW/dapr-traffic-control

Here is how i run this demo in docker-compose:
1. open command, enter the folder of this project which contains the docker-compose.yml file.
2. running command `docker compose up`
3. then you view the demos
   - mail: http://localhost:2080
   - center logs: http://localhost:5340

you can use the `docker logs` to check every service logs in container.

in the demo, the dapr and the service are running in sidecar mode.

The following tow service are running in two containers, but share one ip address.
The sidecar in docker `network_mode: "service:vehicleregistrationservice"`.
```yml
  vehicleregistrationservice:
    image: ${REGISTRY:-dtc}/vehicleregistrationservice:${TAG:-latest}
    build:
      context: .
      dockerfile: src/vehicleregistrationservice/Dockerfile
    environment:
      - SeqServerUrl=http://seq
    ports:
      - "6002:6002"

  vehicleregistrationservice-dapr:
    image: "daprio/daprd:1.8.4"
    network_mode: "service:vehicleregistrationservice"
    depends_on:
      - vehicleregistrationservice
    command: ["./daprd",
      "-app-id", "vehicleregistrationservice",
      "-app-port", "6002",
      "-components-path", "/components",
      "-config", "/config/config.yaml",
      "--log-level", "debug",
      "--enable-api-logging",
      "--enable-profiling"
      ]
    volumes:
      - "./src/dapr/components/:/components"
      - "./src/dapr/config/:/config"
```


```
ApplicationName="TrafficControlService" and @Message like '%Lost track%'
```