import { defineConfig } from 'vitepress'

export default defineConfig({
  title: 'XmlTvSharp',
  description: 'A high-performance reader and writer for .NET.',
  base: '/XmlTvSharp/',
  cleanUrls: true,
  lastUpdated: true,
  themeConfig: {
    nav: [
      { text: 'Guide', link: '/getting-started' },
      { text: 'NuGet', link: 'https://www.nuget.org/packages/XmlTvSharp' }
    ],
    sidebar: [
      {
        text: 'Guide',
        items: [
          { text: 'Getting Started', link: '/getting-started' },
          { text: 'Reading', link: '/reading' },
          { text: 'Writing', link: '/writing' },
          { text: 'Filtering', link: '/filtering' },
          { text: 'Date and Time', link: '/date-time' }
        ]
      },
      {
        text: 'Configuration',
        items: [
          { text: 'Reader Options', link: '/reader-options' },
          { text: 'Writer Options', link: '/writer-options' },
          { text: 'Compatibility Profiles', link: '/compatibility-profiles' }
        ]
      },
      {
        text: 'Reference',
        items: [
          { text: 'Models', link: '/models' },
          { text: 'Error Handling', link: '/error-handling' },
          { text: 'Migrating to v2', link: '/migration-v2' }
        ]
      }
    ],
    socialLinks: [
      { icon: 'github', link: 'https://github.com/eddami/XmlTvSharp' }
    ],
    outline: {
      label: 'On this page'
    },
    footer: {
      message: 'Released under the MIT License.',
      copyright: 'Copyright © 2026 XmlTvSharp'
    },
    search: {
      provider: 'local',
      options: {
        translations: {
          button: {
            buttonText: 'Search',
            buttonAriaLabel: 'Search docs'
          }
        }
      }
    }
  }
})
