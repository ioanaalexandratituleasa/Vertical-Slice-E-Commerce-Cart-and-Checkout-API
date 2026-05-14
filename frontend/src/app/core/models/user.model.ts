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

export interface LoginRequest {
  email: string;
  password: string;
}

export interface LoginResponse {
  token: string;
  userID: number;
  fName: string;
  email: string;
}