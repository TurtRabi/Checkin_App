<template>
      <header class="header">
        <div class="container">
          <div class="logo">
            <img src="@/assets/Logo.png" alt="Logo" />
            <router-link to="/introduce">Google Check-in</router-link>
          </div>
          <nav class="nav" v-if="authStore.isLoggedIn">
            <ul>
              <li><router-link to="/">Home</router-link></li>
              <li><router-link to="/contact">Contact</router-link></li>
              <li><router-link to="/services">Services</router-link></li>
            </ul>
          </nav>
          <div class="auth-buttons">
            <template v-if="!authStore.isLoggedIn">
              <router-link to="/login" class="nav-button">Login</router-link>
              <router-link to="/register" class="nav-button" @click="createRipple">Register</router-link>
            </template>
            <template v-else>
              <button class="nav-button" @click="(e) => { authStore.logout(); createRipple(e); }">Logout</button>
            </template>
          </div>
        </div>
      </header>
    </template>

    <script setup>
    import { useAuthStore } from '@/application/stores/auth';
    const authStore = useAuthStore();

    function createRipple(event) {
      const button = event.currentTarget;
      const circle = document.createElement("span");
      const diameter = Math.max(button.clientWidth, button.clientHeight);
      const radius = diameter / 2;

      circle.style.width = circle.style.height = `${diameter}px`;
      circle.style.left = `${event.clientX - button.offsetLeft - radius}px`;
      circle.style.top = `${event.clientY - button.offsetTop - radius}px`;
      circle.classList.add("ripple");

      const ripple = button.getElementsByClassName("ripple")[0];

      if (ripple) {
        ripple.remove();
      }

      button.appendChild(circle);
    }
    </script>

    <style scoped>
    :root {
        --ease-out-cubic: cubic-bezier(0.215, 0.610, 0.355, 1);
    }

    .header {
      background-color: #1fcd7c;
      color: white;
      padding: 0;
      box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
    }

    .container {
      display: flex;
      justify-content: space-between;
      align-items: center;
      max-width: 1200px;
      margin: 0 auto;
      padding: 0 1rem;
    }

    .logo {
      display: flex;
      align-items: center;
      animation: slideInLeft 1s var(--ease-out-cubic);
    }

    .logo img {
      width: 100px;
      height: 100px;
      transition: transform 0.4s var(--ease-out-cubic);
    }

    .logo img:hover {
      transform: rotate(-15deg);
    }

    .logo a, .logo router-link {
      color: white;
      text-decoration: none;
      font-size: 1.5rem;
      font-weight: bold;
      margin-left: 1rem;
    }

    .nav {
      animation: fadeIn 1s ease-in-out;
    }

    .nav ul {
      list-style: none;
      display: flex;
      margin: 0;
      padding: 0;
    }

    .nav li {
      margin: 0 0.5rem;
    }

    .nav ul li a, .nav ul li router-link {
      color: white;
      text-decoration: none;
      font-size: 1.1rem;
      font-weight: 500;
      padding: 1rem 0.5rem;
      position: relative;
    }

    .nav ul li a::after, .nav ul li router-link::after {
      content: '';
      position: absolute;
      width: 100%;
      height: 2px;
      bottom: 0.5rem;
      left: 0;
      background-color: #fff;
      transform: scaleX(0);
      transform-origin: center;
      transition: transform 0.4s var(--ease-out-cubic);
    }

    .nav ul li a:hover::after, .nav ul li router-link:hover::after {
        transform: scaleX(1);
    }

    .auth-buttons {
      display: flex;
      gap: 1rem;
      animation: slideInRight 1s var(--ease-out-cubic);
    }

    .nav-button {
      position: relative;
      overflow: hidden;
      background-color: #42b983;
      color: white;
      border: none;
      padding: 0.7rem 1.5rem;
      border-radius: 8px;
      cursor: pointer;
      font-size: 1rem;
      transition: all 0.3s var(--ease-out-cubic);
      box-shadow: 0 4px 15px rgba(0,0,0,0.2);
    }

    .nav-button:hover {
      background-color: #36a473;
      transform: translateY(-3px) scale(1.05) rotate(-2deg);
      box-shadow: 0 7px 20px rgba(0,0,0,0.25);
    }

    .nav-button:active {
        transform: translateY(-1px) scale(1.02) rotate(-1deg);
        box-shadow: 0 4px 10px rgba(0,0,0,0.2);
    }

    span.ripple {
      position: absolute;
      border-radius: 50%;
      transform: scale(0);
      animation: ripple 600ms linear;
      background-color: rgba(255, 255, 255, 0.7);
    }

    @keyframes ripple {
      to {
        transform: scale(4);
        opacity: 0;
      }
    }

    @keyframes slideInLeft {
      from {
        transform: translateX(-100%);
        opacity: 0;
      }
      to {
        transform: translateX(0);
        opacity: 1;
      }
    }

    @keyframes slideInRight {
      from {
        transform: translateX(100%);
        opacity: 0;
      }
      to {
        transform: translateX(0);
        opacity: 1;
      }
    }

    @keyframes fadeIn {
      from {
        opacity: 0;
      }
      to {
        opacity: 1;
      }
    }
    </style>
