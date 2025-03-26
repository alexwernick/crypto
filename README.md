# crypto

This is a simple blockchain-based application built in C#. The application simulates a basic blockchain with functionality for creating blocks, validating the chain, and handling transactions. This initial functionality was built in tandem with the course **"Blockchain A-Z™: Build Your Own Blockchain, Cryptocurrency, and Smart Contracts"** but in C#, not in Python like the course sets out. Further functionality was added to allow the code to actually function like a blockchain node i.e. a background service which checks its own knowledge with other nodes in the network. There is a docker compose file which allows you to see this happening in action.

---

## Features

- **Blockchain Implementation**: A custom blockchain with proof-of-work (PoW) consensus.
- **Transaction Management**: Add and manage transactions in the blockchain.
- **Validation**: Ensure the integrity and validity of the chain.
- **Basic API Endpoints**: Interact with the blockchain via RESTful API endpoints.
- **Background Service**: Service which checks its own knowledge with other nodes in the network.

---

## Table of Contents

1. [Getting Started](#getting-started)
2. [Endpoints](#endpoints)
3. [Prerequisites](#prerequisites)
4. [Installation](#installation)
5. [Usage](#usage)
6. [Contributing](#contributing)
7. [Acknowledgments](#acknowledgments)

---

## Getting Started

This blockchain application is designed to demonstrate the fundamentals of blockchain technology, including how blocks are created, mined, and added to a chain. Transactions between users are recorded and validated as part of the blockchain.

---

## API Endpoints

The application exposes the following API endpoints:

### **1. Get Blockchain**
- **Endpoint**: `/blockchain`
- **Method**: GET  
- **Description**: Retrieves the entire blockchain.  
- **Response**:
  ```json
  {
    "blocks": [ ... ],
  }
  ```

---

### **2. Mine Block**
- **Endpoint**: `/blockchain/mine-block`
- **Method**: POST  
- **Description**: Mines a new block using proof-of-work and adds it to the blockchain.  
- **Response**:
  ```json
  {
    "proof": 123456,
    "previousHash": "0000000000000000000abc123def456gh789ijklmnopqrs9876543210",
    "createdDate": "2025-03-26T12:34:56",
    "transactions": [
        {
        "sender": "Alice",
        "receiver": "Bob",
        "amount": 50.0,
        "id": "550e8400-e29b-41d4-a716-446655440000"
        },
        {
        "sender": "Charlie",
        "receiver": "Dave",
        "amount": 25.5,
        "id": "550e8400-e29b-41d4-a716-446655440001"
        }
    ]
  }
  ```

---

### **3. Validate Blockchain**
- **Endpoint**: `/blockchain/is-chain-valid`
- **Method**: GET  
- **Description**: Validates the blockchain to ensure its integrity.  
- **Response**:
  ```json
  {
    "is_chain_valid": true
  }
  ```

---

### **4. Add Transaction**
- **Endpoint**: `/blockchain/add-transaction`
- **Method**: POST  
- **Description**: Adds a new transaction to the memory pool (mempool).  
- **Request Body**:
  ```json
  {
    "sender": "Alice",
    "receiver": "Bob",
    "amount": 31.4
  }
  ```
---

### **5. Add Node**
- **Endpoint**: `/blockchain/add-node`
- **Method**: POST  
- **Description**: Registers a new node to the blockchain network.  
- **Request Body**:
  ```json
  {
    "node_address": "http://127.0.0.1/node-4"
  }
  ```
---

### **6. Get Nodes**
- **Endpoint**: `/blockchain/get-nodes`
- **Method**: GET  
- **Description**: Retrieves the list of nodes in the network.  
- **Response**:
  ```json
  {
    "node_addresses": [ ... ]
  }
  ```

---

### **7. Get Memory Pool**
- **Endpoint**: `/blockchain/get-mem-pool`
- **Method**: GET  
- **Description**: Retrieves all transactions in the memory pool (not yet mined into a block).  
- **Response**:
  ```json
  {
    "transactions": [ ... ]
  }
  ```

---

## Prerequisites

To run this application, ensure you have the following installed:

- **.NET SDK**: Download from [Microsoft .NET](https://dotnet.microsoft.com/).
- **Postman** or equivalent tool for testing API endpoints.
- **C# IDE**: Visual Studio or another preferred IDE.

---

## Installation

1. Clone the repository:
   ```bash
   git clone https://github.com/alexwernick/crypto.git
   cd crypto
   ```

2. Open the project in your IDE (e.g., Visual Studio).

3. Restore dependencies:
   ```bash
   dotnet restore
   ```

4. Build the project:
   ```bash
   dotnet build
   ```

5. Run the application:
   ```bash
   dotnet run
   ```

---

## Usage

1. Start the application by running the project.
2. Use tools like **Postman** or **cURL** to interact with the API endpoints listed above.
3. Test creating transactions, mining blocks, and validating the chain.

---

## Contributing

Contributions are welcome! To contribute:

1. Fork the repository.
2. Create a new branch for your feature:
   ```bash
   git checkout -b feature-name
   ```
3. Commit your changes and push the branch:
   ```bash
   git push origin feature-name
   ```
4. Open a pull request, and describe the changes you've made.

---

## Acknowledgments

- **Udemy Course**: Blockchain A-Z™ by Hadelin de Ponteves, Kirill Eremenko, and SuperDataScience.
- **OpenAI**: For assisting in generating this README.
