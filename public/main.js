document.getElementById('registerPetForm').addEventListener('submit', async function(event) {
    event.preventDefault();
    const petName = document.getElementById('petName').value;
    const petType = document.getElementById('petType').value;

    const response = await fetch('https://waqqly-function.azurewebsites.net/api/HandleRegistration', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({ type: 'pet', petName: petName, petType: petType })
    });

    if (response.ok) {
        alert('Pet registered successfully!');
        fetchPets();
        document.getElementById('registerPetForm').reset(); // Clear the form
    } else {
        alert('Error registering pet.');
    }
});

document.getElementById('registerWalkerForm').addEventListener('submit', async function(event) {
    event.preventDefault();
    const walkerName = document.getElementById('walkerName').value;
    const walkerPhone = document.getElementById('walkerPhone').value;
    const walkerLocation = document.getElementById('walkerLocation').value;

    const response = await fetch('https://waqqly-function.azurewebsites.net/api/HandleRegistration', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({ type: 'walker', walkerName: walkerName, walkerPhone: walkerPhone, walkerLocation: walkerLocation })
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
        listItem.addEventListener('click', () => showDetails(pet.petName, pet.petType));
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
        listItem.textContent = `${walker.walkerName}`;
        listItem.addEventListener('click', () => showDetails(walker.walkerName, walker.walkerPhone, walker.walkerLocation));
        walkerList.appendChild(listItem);
    });
}

function showDetails(...details) {
    const modal = document.getElementById('modal');
    const modalDetails = document.getElementById('modalDetails');
    modalDetails.innerHTML = details.map(detail => `<p>${detail}</p>`).join('');
    modal.classList.add('show');
    document.querySelector('.modal-content').classList.add('show');
}

document.addEventListener('DOMContentLoaded', function() {
    fetchPets();
    fetchWalkers();
});

const modal = document.getElementById('modal');
const closeBtn = document.getElementsByClassName('close')[0];

closeBtn.onclick = function() {
    modal.classList.remove('show');
    document.querySelector('.modal-content').classList.remove('show');
}

window.onclick = function(event) {
    if (event.target == modal) {
        modal.classList.remove('show');
        document.querySelector('.modal-content').classList.remove('show');
    }
}

