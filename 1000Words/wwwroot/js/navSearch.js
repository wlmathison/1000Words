const navSearchBar = document.getElementById("navSearchInput");
const inputGroupSelect = document.getElementById("inputGroupSelect");

inputGroupSelect.addEventListener("click", selectSearch);

function selectSearch() {
    switch (inputGroupSelect.value) {
        case "1":
            navSearchBar.type = "text";
            navSearchBar.value = "";
            navSearchBar.placeholder = "Enter keywords";
            break;

        case "2":
            navSearchBar.type = "date";
            navSearchBar.format = "mm/dd/yyyy";
            break;

        case "3":
            navSearchBar.type = "month";
            break;

        case "4":
            navSearchBar.type = "number";
            navSearchBar.placeholder = "";
            navSearchBar.min = 1900;
            navSearchBar.max = new Date().getFullYear();
            break;
    }
}