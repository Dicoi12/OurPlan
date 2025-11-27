export interface IEventModel {
  id: number;
  title: string;
  startDate: Date;
  endDate: Date;
  createdByUserId: number;
  isShared: boolean;
}

export interface IGroupModel {
  id: number;
  name: string;
  createdByUserId: number;
  userGroups: IUserGroup[];
}

export interface IGroupTokenModel {
  id: number;
  groupId: number;
  token: string;
  expiryDate: Date;
}

export interface ILoginModel {
  email: string;
  password: string;
}

export interface IRegisterModel {
  username: string;
  email: string;
  password: string;
}

export interface IUserModel {
  id: number;
  username: string;
  email: string;
  createdAt: Date;
}

export interface IUserGroup {
  userId: number;
  groupId: number;
  role: number;
}

export interface IExpenseModel {
  id: number;
  title: string;
  amount: number;
  category: string;
  date: Date;
  createdByUserId: number;
  description?: string;
}