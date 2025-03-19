/*!

=========================================================
* Argon Dashboard PRO React - v1.2.5
=========================================================

* Product Page: https://www.creative-tim.com/product/argon-dashboard-pro-react
* Copyright 2024 Creative Tim (https://www.creative-tim.com)

* Coded by Creative Tim

=========================================================

* The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

*/
import Dashboard from "views/pages/Dashboard.js";
import Login from "views/pages/examples/Login.js";
import Register from "views/pages/examples/Register.js";
import Cities from "views/pages/Cities.js";
import CityForm from "views/pages/CityForm.js";
import Customers from "views/pages/Customers.js";
import CustomerForm from "views/pages/CustomerForm.js";
import Products from "views/pages/Products.js";
import ProductForm from "views/pages/ProductForm.js";
import Persons from "views/pages/persons/Persons";
import PersonForm from "views/pages/persons/PersonForm";

const routes = [
  {
    path: "/index",
    name: "Dashboard",
    icon: "ni ni-tv-2 text-primary",
    component: Dashboard,
    layout: "/admin",
    protected: true
  },
  {
    path: "/persons",
    name: "Pessoas",
    icon: "ni ni-building text-info",
    component: Persons,
    layout: "/admin",
    protected: true,
    children: [
      {
        path: "new",
        component: PersonForm
      },
      {
        path: ":id",
        component: PersonForm
      },
      {
        path: ":id/edit",
        component: PersonForm
      }
    ]
  },
  {
    path: "/cities",
    name: "Cidades",
    icon: "ni ni-building text-info",
    component: Cities,
    layout: "/admin",
    protected: true,
    children: [
      {
        path: "create",
        component: CityForm
      },
      {
        path: ":id",
        component: CityForm
      },
      {
        path: ":id/edit",
        component: CityForm
      }
    ]
  },
  {
    path: "/customers",
    name: "Clientes",
    icon: "ni ni-single-02 text-primary",
    component: Customers,
    layout: "/admin",
    protected: true,
    children: [
      {
        path: "new",
        component: CustomerForm
      },
      {
        path: ":id",
        component: CustomerForm
      },
      {
        path: ":id/edit",
        component: CustomerForm
      }
    ]
  },
  {
    path: "/login",
    name: "Login",
    icon: "ni ni-key-25 text-info",
    component: Login,
    layout: "/auth",
    protected: false
  },
  {
    path: "/register",
    name: "Register",
    icon: "ni ni-circle-08 text-pink",
    component: Register,
    layout: "/auth",
    protected: false
  }
];

export default routes;
