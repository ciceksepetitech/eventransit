#!/bin/bash
set -e


GCR_REGISTRY=$GCR_REGISTRY_ID
NAME=ciceksepeti/eventransit
BRANCH=$GITHUB_REF_NAME
BUILDNUMBER=$GITHUB_RUN_NUMBER
TAG=$BRANCH-$BUILDNUMBER


if [ "$BRANCH" = "main"  ]; then
	
    TAG="master-$BUILDNUMBER"
    echo "dockerhub login"
    docker login --username $DOCKER_LOGIN_USER --password $DOCKER_LOGIN_PASS
    echo "docker tag for dockerhub image"
    docker tag $NAME techciceksepeti/eventransit:$TAG
    docker tag $NAME techciceksepeti/eventransit:latest
    echo "docker push to dockerhub"
    docker push techciceksepeti/eventransit:$TAG
    docker push techciceksepeti/eventransit:latest
    echo "dockerhub logout"
    docker logout

fi


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


#gcloud trigger function
gcloud functions call $TRIGGER_FUNCTION --region $TRIGGER_REGION --project $GCR_PROJECT_ID --data '{"imageParent":"ciceksepeti", "imageName":"eventransit", "tag":"'"$TAG"'"}'