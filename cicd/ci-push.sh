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
echo "ecr login"
eval $(aws ecr get-login --region eu-central-1)
echo "docker tag for ecr image"
docker tag $NAME $REMOTE/$NAME:$TAG
echo "docker push to ecr"
docker push $REMOTE/$NAME:$TAG
echo "ecr logout"
docker logout https://$REMOTE


#GCP image push
echo "gcr login"
echo $GCLOUD_SERVICE_KEY | base64 --decode > /tmp/keyfile.json

gcloud auth activate-service-account --key-file=/tmp/keyfile.json

gcloud auth configure-docker
echo "docker tag for gcr image"
docker tag $NAME $GCR_REGISTRY/$NAME:$TAG

echo "docker push to gcr"
docker push $GCR_REGISTRY/$NAME:$TAG
echo "gcr logout"
docker logout https://$GCR_REGISTRY
