import { StatusesEnum } from '../models/statuses.enum';
import { PrioritiesEnum } from '../models/priorities.enum';

export class Check {
  public id: number;
  public host: string;
  public lastCheckTime: Date;
  public statusChangeTime: Date;
  public executionDuration: number;
  public status: StatusesEnum;
  public service: string;
  public checkFullDescription: string;
  public statusInfo: string;
  public description: string;
  public priority: PrioritiesEnum;
  public type: number;
  public loading: boolean = false;
  public environmentId: number;
}
