const { CosmosClient } = require("@azure/cosmos");

const endpoint = process.env.COSMOS_DB_ENDPOINT;
const key = process.env.COSMOS_DB_KEY;
const client = new CosmosClient({ endpoint, key });

module.exports = async function (context, req) {
    context.log('JavaScript HTTP trigger function processed a request.');

    const type = req.query.type || (req.body && req.body.type);

    if (!type) {
        context.res = {
            status: 400,
            body: "Please provide a type (pet or walker) in the query string or in the request body."
        };
        return;
    }

    try {
        let responseMessage;

        if (type === 'pet') {
            responseMessage = await handlePetRegistration(req.body);
        } else if (type === 'walker') {
            responseMessage = await handleWalkerRegistration(req.body);
        } else {
            context.res = {
                status: 400,
                body: "Invalid type provided. Must be 'pet' or 'walker'."
            };
            return;
        }

        context.res = {
            status: 201,
            body: responseMessage
        };

    } catch (error) {
        context.res = {
            status: 500,
            body: `Error: ${error.message}`
        };
    }
};

async function handlePetRegistration(body) {
    const database = client.database('waqqlydog');
    const container = database.container('pets');

    const { petName, petType } = body;
    const newItem = {
        id: `${Date.now()}`,
        petName,
        petType
    };

    const { resource: createdItem } = await container.items.create(newItem);
    return createdItem;
}

async function handleWalkerRegistration(body) {
    const database = client.database('waqqlydog');
    const container = database.container('walkers');

    const { walkerName, walkerPhone } = body;
    const newItem = {
        id: `${Date.now()}`,
        walkerName,
        walkerPhone
    };

    const { resource: createdItem } = await container.items.create(newItem);
    return createdItem;
}
