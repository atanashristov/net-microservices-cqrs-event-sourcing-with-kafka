# net-microservices-cqrs-event-sourcing-with-kafka

Code and notes from studying [.NET Microservices: CQRS & Event Sourcing with Kafka](https://www.udemy.com/course/net-microservices-cqrs-event-sourcing-with-kafka/)

## Section 2: Setup & Structure

### 7. Prerequisites

Install Docker, then verify the installation by checking the installed version:

```sh
docker --version
Docker version 20.10.21-rd, build ac29474
```

Install docker network for this course run:

```sh
docker network create --attachable -d bridge mydockernetwork
```

Verify the network is installed by listing all the networks with:

```sh
docker network ls
NETWORK ID     NAME              DRIVER    SCOPE
ad989e2b5adb   bridge            bridge    local
25f108b98073   host              host      local
a17431149bbd   mydockernetwork   bridge    local
4b3e1aa51681   none              null      local
```

Install Docker Compose, then verify the installation by checking the installed version:

```sh
docker-compose --version
Docker Compose version v2.14.0
```

### 8. Run Kafka in Docker

Run Apache Kafka in Docker using Docker Compose.

Now execute the "docker/docker-compose.yml" file. Run `docker-compose` as detach (`-d`):

```sh
cd docker
docker-compose up -d
time="2023-03-17T16:34:35-05:00" level=warning msg="network default: network.external.name is deprecated in favor of network.name"
[+] Running 0/4
 - zookeeper Pulling                                                                                                                                                                                     13.0s
   - fddf0a981f52 Downloading [=================>    ]  93.63MB/272.9MB                                                                                                     12.2s
 - kafka Pulling                                                                                                                                                                                         13.0s
   - f9587821537a Downloading [=============>        ]   95.3MB/353.4MB
...
Volume "docker_zookeeper_data" Created
Volume "docker_kafka_data"     Created
Container docker-zookeeper-1   Started
Container docker-kafka-1       Started
```

List all docker processes to verify kafka is running:

```sh
 docker ps                                                                                                                pwsh  16:37:17 
CONTAINER ID   IMAGE                       COMMAND                  CREATED          STATUS          PORTS                                                                     NAMES
5cacf3bca639   bitnami/kafka               "/opt/bitnami/script…"   2 minutes ago    Up 2 minutes    0.0.0.0:9092->9092/tcp, :::9092->9092/tcp                                 docker-kafka-1
ea74e0b3da59   bitnami/zookeeper           "/opt/bitnami/script…"   2 minutes ago    Up 2 minutes    2888/tcp, 3888/tcp, 0.0.0.0:2181->2181/tcp, :::2181->2181/tcp, 8080/tcp   docker-zookeeper-1
```

### 9. Run MongoDB in Docker

Run MongoDB in Docker:

```sh
docker run -it -d --name mongo-container -p 27017:27017 --network mydockernetwork --restart always -v mongodb_data_container:/data/db mongo:latest
```

In order to be able to save and persist data we need to specify volume (`-v`). These are directories and files outside the normal union file system and exist as normal directories and files on the host file system.

Verify MongoDB is running:

```sh
docker ps
CONTAINER ID   IMAGE                       COMMAND                  CREATED             STATUS             PORTS                                                                     NAMES
e302b0484ca4   mongo:latest                "docker-entrypoint.s…"   3 minutes ago       Up 3 minutes       0.0.0.0:27017->27017/tcp, :::27017->27017/tcp                             mongo-container
5cacf3bca639   bitnami/kafka               "/opt/bitnami/script…"   14 minutes ago      Up 14 minutes      0.0.0.0:9092->9092/tcp, :::9092->9092/tcp                                 docker-kafka-1
ea74e0b3da59   bitnami/zookeeper           "/opt/bitnami/script…"   14 minutes ago      Up 14 minutes      2888/tcp, 3888/tcp, 0.0.0.0:2181->2181/tcp, :::2181->2181/tcp, 8080/tcp   docker-zookeeper-1
```

Download and install [Robomongo Studio 3T Free](https://robomongo.org/) to access MongoDB

### 10. Run Postgres in Docker

Install Postgesql in Docker:

```sh
docker run -it -d --name postgres-container -p 5432:5432 --network mydockernetwork --restart always -e 'POSTGRES_PASSWORD={password-comes-here}' postgres
```

or use the docker-compose.yaml file:

```sh
```sh
cd docker
docker-compose up -d
```

### 11. Basic project setup

Create core CQRS project:

```sh
cd CQRS-ES
dotnet new classlib -o CQRS.Core
cd ..
```

Create solution and projects in SM (Social Media):

```sh
cd SM-Post
dotnet new sln
dotnet new classlib -o Post.Common 
cd Post.Cmd
dotnet new webapi -o Post.Cmd.Api
dotnet new classlib -o Post.Cmd.Domain
dotnet new classlib -o Post.Cmd.Infrastructure
cd ..
cd Post.Query
dotnet new webapi -o Post.Query.Api
dotnet new classlib -o Post.Query.Domain
dotnet new classlib -o Post.Query.Infrastructure
cd ..
dotnet sln add ../CQRS-ES/CQRS.Core/CQRS.Core.csproj
dotnet sln add ./Post.Cmd/Post.Cmd.Api/Post.Cmd.Api.csproj
... add all projects here ...
```

Add references between the projects:

```sh
dotnet add .\Post.Common\Post.Common.csproj reference ..\CQRS-ES\CQRS.Core\CQRS.Core.csproj     

dotnet add .\Post.Cmd\Post.Cmd.Api\Post.Cmd.Api.csproj reference ..\CQRS-ES\CQRS.Core\CQRS.Core.csproj
dotnet add .\Post.Cmd\Post.Cmd.Api\Post.Cmd.Api.csproj reference .\Post.Cmd\Post.Cmd.Domain\Post.Cmd.Domain.csproj
dotnet add .\Post.Cmd\Post.Cmd.Api\Post.Cmd.Api.csproj reference .\Post.Cmd\Post.Cmd.Infrastructure\Post.Cmd.Infrastructure.csproj
dotnet add .\Post.Cmd\Post.Cmd.Api\Post.Cmd.Api.csproj reference .\Post.Common\Post.Common.csproj

dotnet add .\Post.Cmd\Post.Cmd.Domain\Post.Cmd.Domain.csproj reference ..\CQRS-ES\CQRS.Core\CQRS.Core.csproj
dotnet add .\Post.Cmd\Post.Cmd.Domain\Post.Cmd.Domain.csproj reference .\Post.Common\Post.Common.csproj

dotnet add .\Post.Cmd\Post.Cmd.Infrastructure\Post.Cmd.Infrastructure.csproj reference ..\CQRS-ES\CQRS.Core\CQRS.Core.csproj
dotnet add .\Post.Cmd\Post.Cmd.Infrastructure\Post.Cmd.Infrastructure.csproj reference .\Post.Cmd\Post.Cmd.Domain\Post.Cmd.Domain.csproj

dotnet add .\Post.Cmd\Post.Cmd.Api\Post.Cmd.Api.csproj reference ..\CQRS-ES\CQRS.Core\CQRS.Core.csproj
dotnet add .\Post.Cmd\Post.Cmd.Api\Post.Cmd.Api.csproj reference .\Post.Cmd\Post.Cmd.Domain\Post.Cmd.Domain.csproj
dotnet add .\Post.Cmd\Post.Cmd.Api\Post.Cmd.Api.csproj reference .\Post.Cmd\Post.Cmd.Infrastructure\Post.Cmd.Infrastructure.csproj
dotnet add .\Post.Cmd\Post.Cmd.Api\Post.Cmd.Api.csproj reference .\Post.Common\Post.Common.csproj

dotnet add .\Post.Cmd\Post.Cmd.Domain\Post.Cmd.Domain.csproj reference ..\CQRS-ES\CQRS.Core\CQRS.Core.csproj
dotnet add .\Post.Cmd\Post.Cmd.Domain\Post.Cmd.Domain.csproj reference .\Post.Common\Post.Common.csproj

dotnet add .\Post.Cmd\Post.Cmd.Infrastructure\Post.Cmd.Infrastructure.csproj reference ..\CQRS-ES\CQRS.Core\CQRS.Core.csproj
dotnet add .\Post.Cmd\Post.Cmd.Infrastructure\Post.Cmd.Infrastructure.csproj reference .\Post.Cmd\Post.Cmd.Domain\Post.Cmd.Domain.csproj

dotnet add .\Post.Query\Post.Query.Api\Post.Query.Api.csproj reference ..\CQRS-ES\CQRS.Core\CQRS.Core.csproj
dotnet add .\Post.Query\Post.Query.Api\Post.Query.Api.csproj reference .\Post.Query\Post.Query.Domain\Post.Query.Domain.csproj
dotnet add .\Post.Query\Post.Query.Api\Post.Query.Api.csproj reference .\Post.Query\Post.Query.Infrastructure\Post.Query.Infrastructure.csproj
dotnet add .\Post.Query\Post.Query.Api\Post.Query.Api.csproj reference .\Post.Common\Post.Common.csproj

dotnet add .\Post.Query\Post.Query.Domain\Post.Query.Domain.csproj reference ..\CQRS-ES\CQRS.Core\CQRS.Core.csproj
dotnet add .\Post.Query\Post.Query.Domain\Post.Query.Domain.csproj reference .\Post.Common\Post.Common.csproj

dotnet add .\Post.Query\Post.Query.Infrastructure\Post.Query.Infrastructure.csproj reference ..\CQRS-ES\CQRS.Core\CQRS.Core.csproj
dotnet add .\Post.Query\Post.Query.Infrastructure\Post.Query.Infrastructure.csproj reference .\Post.Query\Post.Query.Domain\Post.Query.Domain.csproj

cd..
```

### 13. NuGet Packages

Install globally EF command line tools:

```sh
dotnet tool install --global dotnet-ef
```

```sh
dotnet add .\CQRS-ES\CQRS.Core\CQRS.Core.csproj package MongoDb.Driver

dotnet add .\SM-Post\Post.Cmd\Post.Cmd.Infrastructure\Post.Cmd.Infrastructure.csproj package Confluent.Kafka
dotnet add .\SM-Post\Post.Cmd\Post.Cmd.Infrastructure\Post.Cmd.Infrastructure.csproj package Microsoft.Extensions.Options
dotnet add .\SM-Post\Post.Cmd\Post.Cmd.Infrastructure\Post.Cmd.Infrastructure.csproj package MongoDB.Driver

dotnet add .\SM-Post\Post.Query\Post.Query.Infrastructure\Post.Query.Infrastructure.csproj package Confluent.Kafka
dotnet add .\SM-Post\Post.Query\Post.Query.Infrastructure\Post.Query.Infrastructure.csproj package Microsoft.EntityFrameworkCore
dotnet add .\SM-Post\Post.Query\Post.Query.Infrastructure\Post.Query.Infrastructure.csproj package Microsoft.EntityFrameworkCore.Design
dotnet add .\SM-Post\Post.Query\Post.Query.Infrastructure\Post.Query.Infrastructure.csproj package Microsoft.EntityFrameworkCore.Tools
dotnet add .\SM-Post\Post.Query\Post.Query.Infrastructure\Post.Query.Infrastructure.csproj package Npgsql.EntityFrameworkCore.PostgreSQL
```

Restore all packages:

```sh
cd .\SM-Post
dotnet restore
```

### 14. Setup debugging configuration (launch.json)

In VS Code press Ctrl+Shift+P and enter ".Net generate assets for Build and Debug". Select "Post.Cmd.Api" as a main project.

Open the `.vscode/launch.json` file. You will see default environment is "Development". Add urls to it. Also rename it as "Post.Cmd.Api"

```json
  "configurations": [
    {
      "name": "Post.Cmd.Api",
...
      "env": {
        "ASPNETCORE_ENVIRONMENT": "Development",
        "ASPNETCORE_URLS": "http://localhost:5010"
      },
```

Copy and paste the entire configuration section as "Post.Query.Api" and give it a different port:

```json
...
      "name": "Post.Query.Api",
      "program": "${workspaceFolder}/SM-Post/Post.Query/Post.Query.Api/bin/Debug/net7.0/Post.Query.Api.dll",
      "cwd": "${workspaceFolder}/SM-Post/Post.Query/Post.Query.Api",
      "env": {
        "ASPNETCORE_ENVIRONMENT": "Development",
        "ASPNETCORE_URLS": "http://localhost:5011"
      },
...
```

## Section 3: Messages

In CQRS and Event Sourcing they are 3 important message types:

- Command
- Event
- Queries

Command:

```md
A combination of expressed intent.
Describes an action that you want to be performed.
It contains the information needed to undertake the desired action.
```

Commands are always named with a verb: `NewPostCommand`, `LikePostCommand`, etc.

Event:

```md
Describe something that ocurred in the application.
A typical source of the event is the aggregate.
When something important happens in the aggregate, it will raise an event.
```

Events are named with past-particle verb: `PostCreatedEvent`, `PostLikedEvent`, `CommentAddedEvent`, etc.
