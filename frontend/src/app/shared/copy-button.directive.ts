import { Directive, HostListener, Input } from '@angular/core';
@Directive({ selector: '[copyButton]', standalone: true })
export class CopyButtonDirective {
  @Input('copyButton') text = '';
  @HostListener('click') onClick(){ if(this.text) navigator.clipboard.writeText(this.text); }
}