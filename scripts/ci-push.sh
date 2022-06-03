#!/bin/bash
set -e
IFS=$'\n\t'

GCR_REGISTRY=$GCR_REGISTRY_ID
NAME=ciceksepeti/eventransit
BRANCHNAME=$(git branch | grep \* | cut -d ' ' -f2 | tr '/' '-' | tr '[:upper:]' '[:lower:]' )
BRANCH=${GITHUB_REF##*/}
BUILDNUMBER=$BUILD_NUMBER


TAG=$BRANCHNAME-$BUILDNUMBER

#GCP image push
echo "gcr login"
echo $GCLOUD_SERVICE_KEY

cat /tmp/keyfile.json

gcloud auth activate-service-account --key-file=/tmp/keyfile.json

gcloud auth configure-docker
echo "docker tag for gcr image"
docker tag $NAME $GCR_REGISTRY/$NAME:$TAG

echo "docker push to gcr"
docker push $GCR_REGISTRY/$NAME:$TAG
echo "gcr logout"
docker logout https://$GCR_REGISTRY
