import { Injectable, signal, effect } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class ThemeService {
  private readonly DARK_MODE_KEY = 'darkMode';

  // Signal to track dark mode state (public for template access)
  darkMode = signal<boolean>(this.getInitialDarkMode());

  constructor() {
    // Check if dark class already exists on HTML element (might be from previous session or SSR)
    const htmlHasDarkClass =
      typeof document !== 'undefined' && document.documentElement.classList.contains('dark');

    // Apply initial dark mode state immediately
    const initialDarkMode = this.darkMode();

    // If HTML element state doesn't match our signal, sync them
    if (htmlHasDarkClass !== initialDarkMode) {
      // Sync to what localStorage/system preference says (our signal value)
      this.applyDarkMode(initialDarkMode);
    } else {
      this.applyDarkMode(initialDarkMode);
    }

    // Watch for signal changes and apply dark mode class automatically
    effect(() => {
      const isDark = this.darkMode();
      this.applyDarkMode(isDark);
    });
  }

  /**
   * Apply dark mode class to document element
   */
  private applyDarkMode(isDark: boolean): void {
    if (typeof document !== 'undefined') {
      const htmlElement = document.documentElement;

      // Use toggle with force parameter for cleaner state management
      htmlElement.classList.toggle('dark', isDark);

      // Verify the class state
      const hasDarkClass = htmlElement.classList.contains('dark');

      // If there's still a mismatch, force it
      if (isDark !== hasDarkClass) {
        console.warn('⚠️ Mismatch detected! Forcing class...');
        if (isDark) {
          htmlElement.classList.add('dark');
        } else {
          htmlElement.classList.remove('dark');
        }
      }
    }
  }

  /**
   * Get initial dark mode state from localStorage or system preference
   */
  private getInitialDarkMode(): boolean {
    // Check localStorage first (user's explicit preference)
    if (typeof localStorage !== 'undefined') {
      const stored = localStorage.getItem(this.DARK_MODE_KEY);
      if (stored !== null) {
        const value = stored === 'true';
        return value;
      }
    }

    // If no localStorage preference, check system preference
    if (typeof window !== 'undefined' && window.matchMedia) {
      const systemPrefersDark = window.matchMedia('(prefers-color-scheme: dark)').matches;
      // Don't save system preference to localStorage - let user toggle first
      return systemPrefersDark;
    }

    return false;
  }

  /**
   * Toggle dark mode
   */
  toggleDarkMode(): void {
    const newValue = !this.darkMode();
    this.darkMode.set(newValue);
    this.applyDarkMode(newValue);
    this.savePreference(newValue);
  }

  /**
   * Set dark mode explicitly
   */
  setDarkMode(isDark: boolean): void {
    this.darkMode.set(isDark);
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
    return this.darkMode();
  }
}
