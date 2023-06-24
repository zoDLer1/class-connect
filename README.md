<p align="center">
<h1 align="center">practice-web</h1>
<div align="center">An app for creating assignments, grading, and sending works between students and teachers.</div>
<div align="center">
    <br />
    <img src="https://github.com/zoDLer1/practice-web/assets/73535285/7dcbe4da-58bd-419b-a530-1eaebfd122d9" width="720" />
</div>
</p>

## Prerequisites
Before you begin, ensure that you have the following prerequisites installed:
- [Git](https://git-scm.com/downloads)
- [.NET 6 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)
- [MariaDB](https://mariadb.org/download) (make sure the MariaDB server is up and running)
- [Node.js](https://nodejs.org/en/download) (comes with [npm](https://www.npmjs.com))
## Getting Started
To run this application, follow these steps:
### 1. Clone the Repository
Open a terminal or command prompt and clone the repository using the following command:
```bash
git clone https://github.com/zoDLer1/class-connect.git
```
### 2. Install Dependencies
Navigate to the project's root directory using a terminal or command prompt:
```bash
cd class-connect
```
#### Backend
Install the backend dependencies by running the following commands:
```bash
cd ClassConnectBack
dotnet restore
```
#### Frontend
Navigate to the frontend folder and install the dependencies by running the following commands:
```bash
cd react-front
npm install
```
### 3. Set Up the Database
Once the MariaDB server is configured, update the connection string in the `appsettings.Development.json` file located in the `ClassConnectBack` folder of the project. Replace `user id` and `password` values with your MariaDB server details.
```json
"ConnectionStrings": {
    "DefaultConnection": "server=localhost;user id=root;password=root;database=planner.mariadb"
}
```
### 4. Build and Run the Application
#### Backend
To build and run the backend application, execute the following command in the `ClassConnectBack` folder:
```bash
dotnet run
```
Wait for the command to complete. Once the backend application has started and created the database, you will need to enter the administrator's email and password.
#### Frontend
To build and run the React.js frontend, execute the following command in the `react-front` folder:
```bash
npm start
```
Wait for the command to complete. Once the frontend application has started, you should see output indicating that the development server is running.
### 5. Access the Application
Open your web browser and navigate to `http://localhost:3000`. You should see the frontend of the application. The frontend will communicate with the backend API running at `https://localhost:7222`.
## Authors

- [@sdywa](https://github.com/sdywa) - Backend
- [@zoDLer](https://github.com/zoDLer1) - Frontend
