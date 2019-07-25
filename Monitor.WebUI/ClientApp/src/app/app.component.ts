import { Component, ChangeDetectionStrategy } from '@angular/core';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class AppComponent {

  private _selectedEnv: string;

  public environments: string[] = ["prod", "beta"];

  public activate(environment: string): void {
    this._selectedEnv = environment;
  }

  public getClass(environment: string): string {
    return this._selectedEnv == environment ? "nav-link active" : "nav-link";
  }

  public getLink(environment: string): string[] {
    return ['/monitor/' + environment + '/checks'];
  }
}
