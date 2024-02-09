echo "Wait for Kibana API.";

$connector_body = '{
   "name": "elasticsearch-connector",
   "config": {
     "connector.class": "io.confluent.connect.elasticsearch.ElasticsearchSinkConnector",
     "tasks.max": "1",
     "topics": "dev.general.logcreation.elasticsearchLogobject.v1.json",
     "key.ignore": "true",
     "schema.ignore": "true",
     "connection.url": "http://elasticsearch:9200",
     "type.name": "elasticsearchLogobject",
     "name": "elasticsearch-connector"
   }
 }';

do {
	$status_code = (Invoke-WebRequest -Uri http://kibana:5601/api/status).StatusCode;
	echo "Kibana status $status_code";
	sleep 10;
} while($status_code -ne 200);

echo "Create elasticsearch connector on kafka connect";

$headers = @{
    'Accept' = 'application/json';
    'Content-Type' = 'application/json';
}

do {
	$status_code = (Invoke-WebRequest -Headers $headers -Method Post -Uri http://kafka-connect:8083/connectors -Body $connector_body).StatusCode;
	sleep 10;
} while ($status_code -ne 200);

echo "Create elasticsearch index";

do {
	$status_code = (Invoke-WebRequest -ContentType application/json -Method Put -Uri http://host.docker.internal:9200/dev.general.logcreation.elasticsearchlogobject.v1.json/ -Body '{"mappings":{"properties":{"Properties":{"properties":{"StatusCode":{"type":"text"}}}}}}').StatusCode;
	sleep 10;
} while ($status_code -ne 200);

echo "Create elasticsearch index pattern";

$headers = @{
    'kbn-xsrf' = 'this_is_required_header';
}

do {
 $status_code = (Invoke-WebRequest -ContentType application/json -Headers $headers -Method Post -Uri http://host.docker.internal:5601/api/saved_objects/index-pattern/logs*?overwrite=true -Body '{"attributes":{"title":"dev.general.logcreation.elasticsearchlogobject.v1.json*","timeFieldName":"Timestamp"}}').StatusCode
 sleep 10;
} while ($status_code -ne 200);
 