export interface IToken{
    token:string;
}
export interface IServiceResult<T>{
    result:T;
    success:boolean;
    message?:string;
}