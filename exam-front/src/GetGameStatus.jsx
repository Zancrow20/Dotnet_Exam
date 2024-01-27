export const getGameStatus = (status) => {
    switch (status){
        case (0): return "New";
        case (1): return "Started";
        case (2): return "Finished";
    }
}