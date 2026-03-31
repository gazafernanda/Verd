/** @type {import('tailwindcss').Config} */
export default {
  content: [
    "./index.html",
    "./src/**/*.{vue,js,ts,jsx,tsx}",
  ],
  theme: {
    extend: {
      colors: {
        'bg-app':             '#f6f8fb',
        'surface':            '#ffffff',
        'primary':            '#1a5641',
        'primary-hover':      '#124030',
        'accent-green':       '#299c77',
        'accent-green-hover': '#228263',
        'success-green':      '#37b27e',
        'light-green-bg':     '#e6f3ef',
        'text-main':          '#182721',
        'text-muted':         '#73847e',
        'text-light':         '#9caaa4',
        'border':             '#e9eceb',
      },
      borderRadius: {
        'sm':  '8px',
        'md':  '16px',
        'lg':  '24px',
        'xl':  '32px',
        '2xl': '40px',
      },
      boxShadow: {
        'sm': '0 2px 8px rgba(0,0,0,0.04)',
        'md': '0 8px 24px rgba(0,0,0,0.06)',
        'lg': '0 16px 32px rgba(26,86,65,0.08)',
      },
      fontFamily: {
        'jakarta': ['"Plus Jakarta Sans"', 'sans-serif'],
      },
      keyframes: {
        'pulse-dot': {
          '0%,100%': { transform: 'scale(0.95)', boxShadow: '0 0 0 0 rgba(239,68,68,0.7)' },
          '70%':     { transform: 'scale(1)',    boxShadow: '0 0 0 4px rgba(239,68,68,0)' },
        },
        'ping-slow': {
          '0%':   { transform: 'scale(0.5)', opacity: '0.8' },
          '100%': { transform: 'scale(1.2)', opacity: '0' },
        },
      },
      animation: {
        'pulse-dot': 'pulse-dot 2s infinite',
        'ping-slow': 'ping-slow 2s linear infinite',
      },
    },
  },
  plugins: [],
}
