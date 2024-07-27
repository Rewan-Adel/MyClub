// get the navbar buttons and content sections
const membersBtn = document.getElementById('members-btn');
const trainersBtn = document.getElementById('trainers-btn');
const offersBtn = document.getElementById('offers-btn');
const servicesBtn = document.getElementById('services-btn');
const actionsBtn = document.getElementById('actions-btn');
const reportsBtn = document.getElementById('reports-btn');
const membersContent = document.getElementById('members-content');
const trainersContent = document.getElementById('trainers-content');
const offersContent = document.getElementById('offers-content');
const servicesContent = document.getElementById('services-content');
const actionsContent = document.getElementById('actions-content');
const reportsContent = document.getElementById('reports-content');

// function to hide all content sections
function hideAllContentSections() {
  membersContent.classList.remove('show');
  trainersContent.classList.remove('show');
  offersContent.classList.remove('show');
  servicesContent.classList.remove('show');
  actionsContent.classList.remove('show');
  reportsContent.classList.remove('show');
}

// add event listeners to the navbar buttons
membersBtn.addEventListener('click', () => {
  hideAllContentSections();
  membersContent.classList.add('show');
});

trainersBtn.addEventListener('click', () => {
  hideAllContentSections();
  trainersContent.classList.add('show');
});

offersBtn.addEventListener('click', () => {
  hideAllContentSections();
  offersContent.classList.add('show');
});

servicesBtn.addEventListener('click', () => {
  hideAllContentSections();
  servicesContent.classList.add('show');
});

actionsBtn.addEventListener('click', () => {
  hideAllContentSections();
  actionsContent.classList.add('show');
});

reportsBtn.addEventListener('click', () => {
  hideAllContentSections();
  reportsContent.classList.add('show');
});