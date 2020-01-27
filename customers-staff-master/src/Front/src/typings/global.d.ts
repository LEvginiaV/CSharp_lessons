declare type Nullable<T> = null | undefined | T;
declare type OmitProperty<T, K extends keyof T> = Pick<T, Exclude<keyof T, K>>;
declare type TimeoutID = number;
