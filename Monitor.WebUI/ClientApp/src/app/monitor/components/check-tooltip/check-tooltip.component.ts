import { Component, Input } from "@angular/core";

@Component({
  selector: 'check-tooltip',
  templateUrl: './check-tooltip.component.html',
  styleUrls: ['./check-tooltip.component.css']
})
export class CheckTooltipComponent {

  private showTooltip: boolean = false;

  @Input() mainText: string = "";
  @Input() tooltipText: string = "";

  public displayTooltip(): void {
    this.showTooltip = true;
  }

  public hideTooltip(): void {
    this.showTooltip = false;
  }
}
