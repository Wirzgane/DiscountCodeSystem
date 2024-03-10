# Discount Code System

The Discount Code System is a server-client application designed for generating and using discount codes. It consists of a server part responsible for generating and managing discount codes and a client part for interacting with the server to generate new codes or use existing ones.

## Features

- Generate unique discount codes of customizable length and count.
- Use generated discount codes.
- Communicate between server and client via TCP protocol.
- Process multiple requests in parallel.

## Requirements

- .NET 8.0 SDK

## Usage

1. Build and run the `DiscountCodeSystem.Worker` project to start the server.
2. Build and run the `DiscountCodeSystem.Client` project to start the client.
3. Follow the prompts on the client console to generate codes, use codes, or request codes.
4. (Optional) Use `<Multiple Startup Projects>` in Visual Studio to debug the two projects simultaneously.

## Project Structure

- `DiscountCodeSystem.Worker`: Contains the server part of the application.
- `DiscountCodeSystem.Client`: Contains the client part of the application.
- `DiscountCodeSystem.Worker.Services`: Contains service classes responsible for generating and managing discount codes.
- `DiscountCodeSystem.Worker.Infrastructure`: Contains the database context and entity classes for storing discount codes.
