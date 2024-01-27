export const getSimbol = (status) => {
    switch (status){
        case (0): return "Камень";
        case (1): return "Ножницы";
        case (2): return "Бумага";
        case (10):return "Будет выбран рандомный символ";
    }
}