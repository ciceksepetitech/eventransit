#!/bin/bash
set -e
IFS=$'\n\t'

AWS_REGISTRY=$AWS_REGISTRY_ID
GCR_REGISTRY=$GCR_REGISTRY_ID
NAME=ciceksepeti/eventransit
BRANCHNAME=$(git branch | grep \* | cut -d ' ' -f2 | tr '/' '-' | tr '[:upper:]' '[:lower:]' )
BRANCH=${GITHUB_REF##*/}
BUILDNUMBER=$BUILD_NUMBER


TAG=$BRANCHNAME-$BUILDNUMBER

#AWS image push
eval $(aws ecr get-login --region eu-central-1)

docker tag -f $NAME $REMOTE/$NAME:$TAG

docker push $REMOTE/$NAME:$TAG

docker logout https://$REMOTE


#GCP image push
echo $GCLOUD_SERVICE_KEY | base64 --decode > /tmp/keyfile.json

gcloud auth activate-service-account --key-file=/tmp/keyfile.json

gcloud auth configure-docker

if [[ $(docker --version | awk '{print $3}')  == 17* ]]; then
	docker tag $NAME $GCR_REGISTRY/$NAME:$TAG
else
	docker tag -f $NAME $GCR_REGISTRY/$NAME:$TAG
fi

docker push $GCR_REGISTRY/$NAME:$TAG

docker logout https://$GCR_REGISTRY
