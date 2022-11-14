kubectl apply `
    -f namespace.yaml `
    -f secrets.yaml `
    -f zipkin.yaml `
    -f redis.yaml `
    -f rabbitmq.yaml `
    -f mosquitto.yaml `
    -f maildev.yaml `
    -f dapr-config.yaml `
    -f pubsub-rabbitmq.yaml `
    -f state-redis.yaml `
    -f email.yaml `
    -f entrycam.yaml `
    -f exitcam.yaml `
    -f vehicleregistrationservice.yaml `
    -f finecollectionservice.yaml `
    -f trafficcontrolservice.yaml `
    -f simulation.yaml
