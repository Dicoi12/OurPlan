export interface IEventModel {
  Id: number;
  Title: string;
  StartDate: Date;
  EndDate: Date;
  CreatedByUserId: number;
  IsShared: boolean;
}

export interface IGroupModel {
  Id: number;
  Name: string;
  CreatedByUserId: number;
  UserGropus: UserGroup[];
}

export interface IGroupTokenModel {
  Id: number;
  GroupId: number;
  Token: string;
  ExpiryDate: Date;
}

export interface ILoginModel {
  Email: string;
  Password: string;
}

export interface IRegisterModel {
  Username: string;
  Email: string;
  Password: string;
}

export interface IUserModel {
  Id: number;
  Username: string;
  Email: string;
  CreatedAt: Date;
}
