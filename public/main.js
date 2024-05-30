document.getElementById('pet-form').addEventListener('submit', async (event) => {
    event.preventDefault();
    const petName = document.getElementById('pet-name').value;
    const petType = document.getElementById('pet-type').value;

    const response = await fetch('https://<your-function-app-name>.azurewebsites.net/api/handleRegistration?type=pet', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({ petName, petType })
    });
    const data = await response.json();
    console.log(data);
});

document.getElementById('walker-form').addEventListener('submit', async (event) => {
    event.preventDefault();
    const walkerName = document.getElementById('walker-name').value;
    const walkerPhone = document.getElementById('walker-phone').value;

    const response = await fetch('https://<your-function-app-name>.azurewebsites.net/api/handleRegistration?type=walker', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({ walkerName, walkerPhone })
    });
    const data = await response.json();
    console.log(data);
});
