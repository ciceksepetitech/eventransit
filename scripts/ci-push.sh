#!/bin/bash
set -e


AWS_REGISTRY=$ECR_REGISTRY_ID
GCR_REGISTRY=$GCR_REGISTRY_ID
NAME=ciceksepeti/eventransit
BRANCH=$GITHUB_REF_NAME
BUILDNUMBER=$GITHUB_RUN_NUMBER
TAG=$BRANCH-$BUILDNUMBER


if [ "$BRANCH" = "main"  ]; then
	
    echo "dockerhub login"
    docker login --username $DOCKER_LOGIN_USER --password $DOCKER_LOGIN_PASS
    echo "docker tag for dockerhub image"
    docker tag $NAME techciceksepeti/eventransit:$TAG
    echo "docker push to dockerhub"
    docker push techciceksepeti/eventransit:$TAG
    echo "dockerhub logout"
    docker logout

fi


#AWS image push
echo "ecr login"
eval $(aws ecr get-login --region eu-central-1)
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
