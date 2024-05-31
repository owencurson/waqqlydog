document.getElementById('registerWalkerForm').addEventListener('submit', async function(event) {
    event.preventDefault();
    const walkerName = document.getElementById('walkerName').value;
    const walkerPhone = document.getElementById('walkerPhone').value;

    const response = await fetch('https://waqqly-function.azurewebsites.net/api/HandleRegistration', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({ type: 'walker', walkerName: walkerName, walkerPhone: walkerPhone })
    });

    if (response.ok) {
        alert('Walker registered successfully!');
        fetchWalkers();
        document.getElementById('registerWalkerForm').reset(); // Clear the form
    } else {
        alert('Error registering walker.');
    }
});

async function fetchPets() {
    const response = await fetch('https://waqqly-function.azurewebsites.net/api/GetRegisteredPets');
    const pets = await response.json();
    const petList = document.getElementById('petList');
    petList.innerHTML = '';
    pets.forEach(pet => {
        const listItem = document.createElement('li');
        listItem.textContent = `${pet.petName} (${pet.petType})`;
        petList.appendChild(listItem);
    });
}

async function fetchWalkers() {
    const response = await fetch('https://waqqly-function.azurewebsites.net/api/GetRegisteredWalkers');
    const walkers = await response.json();
    const walkerList = document.getElementById('walkerList');
    walkerList.innerHTML = '';
    walkers.forEach(walker => {
        const listItem = document.createElement('li');
        listItem.textContent = `${walker.walkerName} (${walker.walkerPhone})`;
        walkerList.appendChild(listItem);
    });
}

document.addEventListener('DOMContentLoaded', function() {
    fetchPets();
    fetchWalkers();
});
