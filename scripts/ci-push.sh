#!/bin/bash
set -e


AWS_REGISTRY=$ECR_REGISTRY_ID
GCR_REGISTRY=$GCR_REGISTRY_ID
NAME=ciceksepeti/eventransit
BRANCHNAME=$(git branch | grep \* | cut -d ' ' -f2 | tr '/' '-' | tr '[:upper:]' '[:lower:]' )
BRANCH=${GITHUB_REF##*/}
BUILDNUMBER=$BUILD_NUMBER
TAG=$BRANCHNAME-$BUILDNUMBER



#AWS image push
echo "ecr login"
aws ecr get-login-password --region eu-central-1 | docker login --username AWS --password-stdin $AWS_REGISTRY
echo "docker tag for ecr image"
docker tag $NAME $AWS_REGISTRY/$NAME:$TAG
echo "docker push to ecr"
docker push $AWS_REGISTRY/$NAME:$TAG
echo "ecr logout"
docker logout https://$REMOTE



#GCP image push
echo "gcr login"
echo $GCLOUD_SERVICE_KEY | base64 -di > /tmp/keyfile.json
gcloud auth activate-service-account --key-file=/tmp/keyfile.json
gcloud auth configure-docker
echo "docker tag for gcr image"
docker tag $NAME $GCR_REGISTRY/$NAME:$TAG
echo "docker push to gcr"
docker push $GCR_REGISTRY/$NAME:$TAG
echo "gcr logout"
docker logout https://$GCR_REGISTRY
