import { Component, OnInit, inject } from '@angular/core';
import { RouterLink } from '@angular/router';
import {
  Sparkles,
  Zap,
  Calendar,
  DollarSign,
  Clock,
  TrendingUp,
  Shield,
  ArrowRight,
  Check,
  Sun,
  Moon,
  LUCIDE_ICONS,
  LucideIconProvider,
  LucideAngularModule,
} from 'lucide-angular';
import { ThemeService } from '../../shared/services/theme.service';

@Component({
  selector: 'app-landing-page',
  templateUrl: './landing-page.component.html',
  styleUrls: ['./landing-page.component.scss'],
  imports: [RouterLink, LucideAngularModule],
  providers: [
    {
      provide: LUCIDE_ICONS,
      multi: true,
      useValue: new LucideIconProvider({
        Sparkles,
        Zap,
        Calendar,
        DollarSign,
        Clock,
        TrendingUp,
        Shield,
        ArrowRight,
        Check,
        Sun,
        Moon,
      }),
    },
  ],
})
export class LandingPageComponent implements OnInit {
  protected themeService = inject(ThemeService);

  features = [
    {
      icon: 'dollar-sign',
      title: 'Automated Budgeting',
      description: 'AI-powered agents analyze your spending patterns and automatically create and adjust budgets to help you reach your financial goals.',
    },
    {
      icon: 'calendar',
      title: 'Smart Calendar Management',
      description: 'Let our agents optimize your schedule, suggest time blocks for important tasks, and automatically coordinate with your financial planning.',
    },
    {
      icon: 'zap',
      title: 'Intelligent Financial Planning',
      description: 'Get personalized financial recommendations and automated investment strategies based on your unique lifestyle and goals.',
    },
    {
      icon: 'clock',
      title: 'Lifestyle Automation',
      description: 'Seamlessly integrate your finances with your daily life. Our agents handle the complexity so you can focus on what matters.',
    },
  ];

  benefits = [
    'Save hours every week on financial management',
    'Make smarter financial decisions with AI insights',
    'Stay on top of your schedule automatically',
    'Achieve your financial goals faster',
    'Bank-level security and encryption',
  ];

  constructor() {}

  ngOnInit() {}

  toggleDarkMode() {
    this.themeService.toggleDarkMode();
  }
}
