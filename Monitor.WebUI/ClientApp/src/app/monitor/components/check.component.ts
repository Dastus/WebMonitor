import { Component, Input, ChangeDetectionStrategy, Output, EventEmitter } from '@angular/core';
import { Check } from '../models/check.model';
import { PrioritiesEnum } from '../models/priorities.enum';
import { StatusesEnum } from '../models/statuses.enum';

@Component({
  selector: 'check',
  templateUrl: './check.component.html',
  //changeDetection: ChangeDetectionStrategy.OnPush,
  styles: ['div.row {border-bottom: 1px solid #ddd} div.row:hover {background-color: #ddd; } p { overflow: hidden; text-overflow: ellipsis }']
})
export class CheckComponent {
  @Input() check: Check;
  @Output() onManualRun: EventEmitter<Check> = new EventEmitter<Check>();

  public getPriorityStyle(): Object {
      switch(this.check.priority) {
        case (PrioritiesEnum.Critical):
          return {'background-color': 'red'};
        case (PrioritiesEnum.High):
          return {'background-color': 'orange'};
        case (PrioritiesEnum.Medium):
          return {'background-color': 'yellow'};
        default: 
          return {'background-color': 'green'};
      }
  }
  
  public getPriorityDescription(): string {
    return PrioritiesEnum[this.check.priority];
  }

  public getStatusStyle(): Object {
    switch(this.check.status) {
      case (StatusesEnum.CRITICAL):
        return {'background-color': 'red'};
      case (StatusesEnum.WARNING):
        return {'background-color': 'yellow'};
      default: 
        return {'background-color': 'green'};
    }
  }

  public getStatusDescription(): string {
    return StatusesEnum[this.check.status];
  }

  public runManualCheck(check: Check): void {
    console.log("RUN");//

    this.onManualRun.emit(check);
  }
}
