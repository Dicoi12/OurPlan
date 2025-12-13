export interface IEventModel {
  id: number;
  title: string;
  startDate: Date;
  endDate: Date;
  createdByUserId: number;
  isShared: boolean;
  user?: {
    username: string;
  };
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
  date: Date | string;
  createdByUserId: number;
  description?: string;
}

export interface ITaskModel {
  id: number;
  title: string;
  description?: string;
  dueDate?: Date | string;
  priority: "low" | "medium" | "high";
  status: "pending" | "in_progress" | "completed";
  createdByUserId: number;
  groupId?: number;
  isCompleted: boolean;
  createdAt?: Date | string;
  updatedAt?: Date | string;
}