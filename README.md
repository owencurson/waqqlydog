# waqqlydog
Waqq.ly is a web application that helps dog owners identify dog walkers in their area. This project includes a rudimentary frontend for registering pets and dog walkers, a microservices-oriented backend using Azure Functions, and a NoSQL database (Azure Cosmos DB) for storing the data.

Prerequisites - 
Azure account
GitHub account
Node.js and npm installed
Azure CLI installed

Deployment Instructions
Step 1: Clone the Repository
Clone the project repository to your local machine.

bash
Copy code
git clone https://github.com/yourusername/waqqlydog.git
cd waqqlydog
Step 2: Set Up Azure Resources
1. Create an Azure Resource Group
bash
Copy code
az group create --name waqqly-resource-group --location <your-preferred-location>
2. Create Azure Cosmos DB
bash
Copy code
az cosmosdb create --name waqqly-cosmos-db --resource-group waqqly-resource-group --kind GlobalDocumentDB
az cosmosdb sql database create --account-name waqqly-cosmos-db --resource-group waqqly-resource-group --name waqqlydog
az cosmosdb sql container create --account-name waqqly-cosmos-db --resource-group waqqly-resource-group --database-name waqqlydog --name pets --partition-key-path /id
az cosmosdb sql container create --account-name waqqly-cosmos-db --resource-group waqqly-resource-group --database-name waqqlydog --name walkers --partition-key-path /id
3. Create an Azure Function App
bash
Copy code
az functionapp create --resource-group waqqly-resource-group --consumption-plan-location <your-preferred-location> --runtime dotnet --functions-version 4 --name waqqly-function --storage-account <your-storage-account>
4. Set Cosmos DB Connection String in Function App
Get the connection string from the Azure portal:

Navigate to your Cosmos DB instance
Under Settings, click on Keys
Copy the PRIMARY CONNECTION STRING
Set the connection string in the Function App:

bash
Copy code
az functionapp config appsettings set --name waqqly-function --resource-group waqqly-resource-group --settings "COSMOS_DB_CONNECTION_STRING=<your-cosmos-db-connection-string>"
Step 3: Deploy Azure Functions
Navigate to the function directory:

bash
Copy code
cd waqqlydog/Untitled
Publish the function app:

bash
Copy code
func azure functionapp publish waqqly-function
Step 4: Set Up Azure Static Web Apps
Create an Azure Static Web App via the Azure Portal:

Navigate to Static Web Apps.
Click on Create.
Fill in the necessary details:
Resource Group: waqqly-resource-group
Name: waqqly-static-web
Region: Select your preferred region
Deployment Source: GitHub
Repository: Select your GitHub repository
Branch: main
Build Presets: Custom
Custom build command: Leave empty
Output location: /
Configure GitHub Actions:

This step should be automated by Azure Static Web Apps service. Ensure the GitHub Actions workflow file is created in your repository.
Step 5: Update Frontend Configuration
Update the API URLs in main.js to point to your Azure Function endpoints.

main.js:

javascript
Copy code
const API_BASE_URL = 'https://waqqly-function.azurewebsites.net/api';
Step 6: Deploy the Frontend
Commit and push the changes to your GitHub repository.

bash
Copy code
git add .
git commit -m "Updated API URLs"
git push origin main
The GitHub Actions workflow will automatically deploy your frontend to Azure Static Web Apps.

Step 7: Testing the Application
Open your web application at the URL provided by Azure Static Web Apps (e.g., https://<your-app-name>.azurestaticapps.net).

Register pets and dog walkers using the forms.

Verify that registered pets and walkers are displayed correctly.

Click on entries to see more details in a modal popup.

Ensure you can close the modal by clicking the close button or outside the modal to return to the main page.