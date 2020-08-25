# Iarly's Chat

Iarly's Chat is a cloud-ready chat.

The Chat was written in .NET Core and can be executed in multiple server instances.
The diagram below represents the flow of a message through a multi-server environment.

![alt text](https://github.com/iarly/chat/raw/master/doc/model.png "The diagram below represents the flow of a message through a multi-server environment.")

At the border, the nginx control the load-balance of the multiples server instances. 
The communication between each server occurs through the Redis Message Broker.

## Installation

Iarly's Chat requires [.NET core 3.1](https://dotnet.microsoft.com/download) and, optionally, [docker-compose](https://docs.docker.com/compose/) to run.

### Basic installation

Install the dependencies & build...

```sh
$ dotnet restore
$ dotnet build
```

### Running single-instance server

```sh
$ dotnet run --project Chat.Server
```

### Running server multi-instance

Build the docker-compose...

```sh
$ docker-compose build
```

Running Redis, MongoDb and 5 instances of chat-server...

```sh
$ docker-compose up --scale chat-server=5
```

### Running the client

For each client...

```sh
$ dotnet run --project Chat.Client.ConsoleApp
```

If you're using server multi-instance, you must use the nginx port (34000)...

```sh
$ dotnet run --project Chat.Client.ConsoleApp --port 34000
```

## Commands

- /to <nickname> <message>: it sends a public message to another user
- /private <nickname> <message>: it sends a private message to another user
- /room <room>: it changes the current room of user
- /exit: it disconnects the user
  
## (Optional) Client arguments

--host <hostname>: it sets the host of the server (default is localhost)
--port <portnumber>: it sets the port of the server (default is 33000)

## (Optional) Server arguments / environments variables

--host <hostname>: it sets the network interface where the socket is bound (default is localhost)
--port <portnumber: its sets the port number of the server (default 33000)
--redis <connectionString>: it sets the connection string of Redis Server (default is null)*
--mongoDbConnection <connectionString>: it sets the connection stirng of mongo (default is null)*
--mongoDbDatabase <databaseName>: it sets the database of mongo (default is null)*
  
* If you didn't set the redis or mongoDb configuration, the server will use the in-memory and dummy message-broker and it will not work as multi-instance.
  

