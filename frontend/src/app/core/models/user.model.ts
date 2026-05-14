export interface RegisterRequest{
    fName: string;
    lName : string;
    email : string;
    password : string;
}

export interface RegisterResponse{
    userID : number;
    fName : string;
    lName : string;
    eMail : string;
}