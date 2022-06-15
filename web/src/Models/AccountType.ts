import { BaseEntity } from "@/Models/BaseEntity";

export interface AccountTypeCreationArgs {
    type: string
}

export interface AccountType extends AccountTypeCreationArgs, BaseEntity {

}