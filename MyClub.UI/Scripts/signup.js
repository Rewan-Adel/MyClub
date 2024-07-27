const slidePage = document.querySelector(".slide-page");
const nextBtns = document.querySelectorAll(".next");
const prevBtns = document.querySelectorAll(".prev");
const progressText = document.querySelectorAll(".step p");
const progressCheck = document.querySelectorAll(".step .check");
const bullet = document.querySelectorAll(".step .bullet");
let current = 1;

function validateStep(step) {
    let isValid = true;
    const inputs = document.querySelectorAll(`.page:nth-child(${step}) input`);

    inputs.forEach(input => {
        if (input.value.trim() === "") {
            isValid = false;
            input.style.borderColor = "red";
        } else {
            input.style.borderColor = "#e6e6e6";
        }
    });

    return isValid;
}

function navigate(direction) {
    if (direction === "next") {
        if (validateStep(current)) {
            slidePage.style.marginLeft = `${-25 * current}%`;
            bullet[current - 1].classList.add("active");
            progressCheck[current - 1].classList.add("active");
            progressText[current - 1].classList.add("active");
            current += 1;
        }
    } else if (direction === "prev") {
        slidePage.style.marginLeft = `${-25 * (current - 2)}%`;
        bullet[current - 2].classList.remove("active");
        progressCheck[current - 2].classList.remove("active");
        progressText[current - 2].classList.remove("active");
        current -= 1;
    }
}

nextBtns.forEach((btn, index) => {
    btn.addEventListener("click", (event) => {
        event.preventDefault();
        navigate("next");
    });
});

prevBtns.forEach((btn, index) => {
    btn.addEventListener("click", (event) => {
        event.preventDefault();
        navigate("prev");
    });
});