let alert = document.querySelector(".alert");
alert(alert.textContent);

let form = document.querySelector(".wrapper");
let error = document.querySelector("#error");
let password = document.querySelector("#password");
let confirmPass = document.querySelector("#confirmPass");

form.addEventListener("submit", (e) => {
    if (password.value != confirmPass.value) {
        error.innerHTML = `<p style="color:red">Password does not match</p>`;
        e.preventDefault();
    }
})

let price = document.querySelector('#price');
price.addEventListener('input', () => {

    let regex = /^\d+(\.\d{0,2})?$/;
    if (!regex.test(price.value)) {
        // Display an error message or take appropriate action
        console.log('Invalid price format');
        // Optionally, clear the input field or provide feedback to the user
        price.value = '';
    }
});

let quantity = document.querySelector("#quantity");
quantity.addEventListener("input", () => {
    let min = quantity.min;
    if (quantity.value < min)
        quantity.value = min;
})