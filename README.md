# Sales Lab – Dynamics 365 CRM

Projeto de CRM de vendas desenvolvido no Microsoft Dynamics 365, utilizando Dataverse e Model-Driven App.
O objetivo do projeto é simular um cenário real de gestão de clientes, produtos e pedidos, aplicando boas práticas
de modelagem de dados e estruturação de aplicações no Dynamics 365.

---

## 🎯 Objetivo do Projeto

Criar um sistema de CRM funcional para controle de vendas, permitindo:
- Cadastro de clientes
- Cadastro de produtos
- Criação e gerenciamento de pedidos
- Relacionamento entre dados de forma estruturada

Este projeto está sendo desenvolvido com foco em aprendizado prático e construção de portfólio
para vagas de estágio e nível júnior em Dynamics 365.

---

## 🧱 Modelagem de Dados (Dataverse)

### Tabela Cliente
Responsável por armazenar as informações dos clientes.

Campos principais:
- Nome
- Email
- Telefone

### Tabela Produto
Responsável por armazenar os produtos disponíveis para venda.

Campos principais:
- Nome do Produto
- Preço
- Ativo

### Tabela Pedido
Representa uma venda realizada no sistema.

Campos principais:
- Cliente (Lookup)
- Produto (Lookup)
- Quantidade
- Valor Total
- Data do Pedido

---

## 🔗 Relacionamentos

- **Cliente (1:N) Pedido**  
  Um cliente pode possuir vários pedidos.

- **Produto (1:N) Pedido**  
  Um produto pode estar associado a vários pedidos.

---

## 🧩 Model-Driven App

Foi criado um Model-Driven App baseado nas tabelas do Dataverse, permitindo
a navegação e o gerenciamento dos dados de forma padrão do Dynamics 365.

Funcionalidades disponíveis:
- Cadastro e edição de Clientes
- Cadastro e edição de Produtos
- Criação e acompanhamento de Pedidos

---

## ⚙️ Tecnologias Utilizadas

- Microsoft Dynamics 365
- Power Apps
- Dataverse
- Model-Driven App

---

## 📌 Status do Projeto

✔️ Modelagem de dados concluída  
✔️ Relacionamentos configurados  
✔️ Model-Driven App funcional  

🔄 Próximos passos:
- Criação de Business Rules
- Implementação de JavaScript
- Desenvolvimento de Plugin
- Automações com Power Automate

---

## 👤 Autor

Projeto desenvolvido por **Vinicius Ferreira** com foco em aprendizado prático
e desenvolvimento profissional na área de Dynamics 365.
