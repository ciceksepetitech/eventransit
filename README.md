# EvenTransit

EvenTransit is a solution which transfers events into relevant service(s) in a distributed way. Distributed messages handled by built in RabbitMQ implementation.

## Highlights
- Language agnostic way to implement distributed messaging. Best fit for microservices.
- Everything is a simple HTTP request.
- Super easy configuration.
- Connection and channel management.
- Failure management. (Retry logic.)
- Fully compatible with containerized environments such as Kubernetes.

## Dependencies
- [.Net 6.0](https://dotnet.microsoft.com/download/dotnet/6.0)
- [RabbitMq](https://www.rabbitmq.com/download.html)
- [MongoDB](https://www.mongodb.com/try/download/community)

## Message Processing Mechanism

An event is published into EvenTransit service. EvenTransit transfers published message into event's exchange. This exchange delivers message to all bound queues. Each queue consumers make HTTP call with message to external services.


![EvenTransit Message Processing Mechanism](assets/EvenTransitInternalMechanism.svg) 

### System Architecture

![EvenTransit Dependency Relations](assets/EvenTransitWithDependencies.svg)

 
## Retry Mechanism

Retry mechanism is involved occurence of an error from external services. Delivered message is transferred into retry exchange and queue. The message waits 30 sec before it republishes. EvenTransit tries message to publish 5 times. Each HTTP call is logged into database.

![EvenTransit Retry Mechanism](assets/EvenTransitRetryMechanism.svg)

> Used Dead Letter Exchange and Dead Letter Queue patterns

## Installation

To get started locally, follow these instructions:
- Clone to your local computer using `git`
- Make sure that you have `.Net 5.0`
- Run `docker-compose up` command in your shell
  - Navigate to `http://<SERVER_IP_OR_HOST>:5101` for API project
  - Navigate to `http://<SERVER_IP_OR_HOST>:5100` for UI project

## Usage

Publish an event with below command

```bash 
curl -X POST "https://localhost:5010/api/v1/event" \ 
-H  "accept: */*" \
-H  "Content-Type: application/json" \ 
-d "{\"eventName\":\"order_created\",\"payload\":{\"name\":\"test\"}}"
``` 

## Dashboard
[Dashboard Wiki](Dashboard.md)

## Contribution
We are grateful to the community for contributing bugfixes and improvements. Read below to learn how you can take part in improving EvenTransit.

### Contribution Prerequisites
You have installed below prerequisites.
- .Net 5.0
- RabbitMq
- MongoDB


### Sending a Pull Request
Before submitting a pull request, please make sure the following is done:
- Fork the repository and create your branch from `main`.
- Make sure you use [Gitflow](https://www.atlassian.com/git/tutorials/comparing-workflows/gitflow-workflow) branching model 
- Don't forget to add new feature explanation to documentation

## Licenses
EvenTransit use either the [MIT](LICENSE.txt) or [Apache 2](https://www.apache.org/licenses/LICENSE-2.0) licenses for code.

