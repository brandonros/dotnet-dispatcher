# dotnet-message-hub

HTTP microservices that brokers to/from RabbitMQ with MassTransit

## Example request

```
curl -X 'POST' \
  'http://localhost:5001/rpc' \
  -H 'accept: application/json' \
  -H 'Content-Type: application/json' \
  -d '{
  "jsonrpc": "2.0",
  "method": "account.get",
  "id": "a81bc81b-dead-4e5d-abff-90865d1e13b1",
  "params": {"accountId":"a81bc81b-dead-4e5d-abff-90865d1e13b1"}
}'
```
