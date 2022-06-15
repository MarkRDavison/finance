import { BaseEntity } from "@/Models/BaseEntity";

export interface BankCreationArgs {
    name: string
}

export interface Bank extends BankCreationArgs, BaseEntity {

}