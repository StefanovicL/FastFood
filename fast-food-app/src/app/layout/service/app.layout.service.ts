import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';

export interface AppConfig {
    inputStyle: string;
    colorScheme: string;
    theme: string;
    ripple: boolean;
    menuMode: string;
    scale: number;
}

interface LayoutState {
    profileSidebarVisible: boolean;
    configSidebarVisible: boolean;
}

@Injectable({
    providedIn: 'root',
})
export class LayoutService {

    config: AppConfig = {
        ripple: false,
        inputStyle: 'outlined',
        menuMode: 'static',
        colorScheme: 'light',
        theme: 'lara-light-indigo',
        scale: 14,
    };

    state: LayoutState = {
        profileSidebarVisible: false,
        configSidebarVisible: false
    };

    private configUpdate = new Subject<AppConfig>();

    configUpdate$ = this.configUpdate.asObservable();

    showProfileSidebar() {
        this.state.profileSidebarVisible = !this.state.profileSidebarVisible;
    }

    showConfigSidebar() {
        this.state.configSidebarVisible = true;
    }

    onConfigUpdate() {
        this.configUpdate.next(this.config);
    }

}
