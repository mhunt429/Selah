import { Injectable, signal } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class ThemeService {
  private readonly DARK_MODE_KEY = 'darkMode';

  // Signal to track dark mode state
  private _darkMode = signal<boolean>(this.getInitialDarkMode());

  constructor() {
    // Apply initial dark mode state immediately
    const initialDarkMode = this._darkMode();
    this.applyDarkMode(initialDarkMode);
  }

  /**
   * Get dark mode signal (read-only access)
   */
  get darkMode() {
    return this._darkMode.asReadonly();
  }

  /**
   * Apply dark mode class to document element
   */
  private applyDarkMode(isDark: boolean): void {
    if (typeof document !== 'undefined') {
      if (isDark) {
        document.documentElement.classList.add('dark');
      } else {
        document.documentElement.classList.remove('dark');
      }
    }
  }

  /**
   * Get initial dark mode state from localStorage or system preference
   */
  private getInitialDarkMode(): boolean {
    // Check localStorage first
    const stored = localStorage.getItem(this.DARK_MODE_KEY);
    if (stored !== null) {
      return stored === 'true';
    }

    // Check system preference
    if (typeof window !== 'undefined' && window.matchMedia) {
      return window.matchMedia('(prefers-color-scheme: dark)').matches;
    }

    // Default to light mode
    return false;
  }

  /**
   * Toggle dark mode
   */
  toggleDarkMode(): void {
    const newValue = !this._darkMode();
    this._darkMode.set(newValue);
    this.applyDarkMode(newValue);
    this.savePreference(newValue);
  }

  /**
   * Set dark mode explicitly
   */
  setDarkMode(isDark: boolean): void {
    this._darkMode.set(isDark);
    this.applyDarkMode(isDark);
    this.savePreference(isDark);
  }

  /**
   * Save preference to localStorage
   */
  private savePreference(isDark: boolean): void {
    if (typeof localStorage !== 'undefined') {
      localStorage.setItem(this.DARK_MODE_KEY, isDark.toString());
    }
  }

  /**
   * Check if dark mode is currently enabled
   */
  isDarkMode(): boolean {
    return this._darkMode();
  }
}
