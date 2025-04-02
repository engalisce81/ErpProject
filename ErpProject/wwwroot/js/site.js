var drop = document.getElementsByClassName("droplist");
for (let i = 0; i < drop.length; i++) {
    drop[i].firstElementChild.onclick = function () {
        if (drop[i].lastElementChild.classList.contains("up")) {
            drop[i].lastElementChild.classList.replace("up", "down");
            drop[i].firstElementChild.firstElementChild.lastElementChild.id = "down"
        } else if (drop[i].lastElementChild.classList.contains("down")) {
            drop[i].lastElementChild.classList.replace("down", "up");
            drop[i].firstElementChild.firstElementChild.lastElementChild.id = "up";
        }
    }
}


const previousbt = document.getElementsByClassName("previousbt")[0];
const rowperpage = 8;
const rowTable = document.querySelectorAll("tbody tr");
const numberpage = Math.ceil(rowTable.length / rowperpage);
document.addEventListener("DOMContentLoaded", function () {
    for (let i = 1; i <= numberpage; i++) {
        const pageLink = document.createElement('span');
        pageLink.className = "page-item";
        pageLink.textContent = i;
        pageLink.addEventListener('click', function () {
            showpage(i);
        });
        previousbt.appendChild(pageLink);
    }
    showpage(1);
    function showpage(thepagenumber) {
        const start = (thepagenumber - 1) * rowperpage;
        const end = start + rowperpage;
        rowTable.forEach(function (e) {
            e.className = 'hidden';
        });
        for (let i = start; i < end; i++) {

            rowTable[i].className = 'show';
        }
    }

});


   


