import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { LayoutService } from 'src/app/layout/service/app.layout.service';
import { AuthService } from '../auth/auth.service';

@Component({
    selector: 'app-landing',
    templateUrl: './landing.component.html',
    styleUrls: ['./landing.component.scss']
})
export class LandingComponent implements OnInit {
    isSignedIn: boolean = false;
    username: string = '';

    constructor(public layoutService: LayoutService, public router: Router, private authService: AuthService) { }
    
    ngOnInit(): void {
        this.authService.getSignInStatus().subscribe(result => {
            this.isSignedIn = result;
        });

        this.authService.getCurrentUsername().subscribe(result => {
            this.username = result;
        });
    }

    logout() {
        this.authService.logout();
    }
}
