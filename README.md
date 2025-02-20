# 📚 Projeto DemoCrud - Catálogo de Livros

### Aplicação ASP.NET MVC para gerenciamento de um catálogo de livros, com operações de CRUD, paginação e busca dinâmica.
---

## 🚀 Funcionalidades

- 📖 Cadastro, edição e exclusão de livros

- 📃 Visualização de lista paginada

- 🔎 Busca e ordenação dinâmica

- 📝 Modal para criação e edição de livros
---

## ⚙️ Tecnologias Utilizadas

- ASP.NET MVC

- C#

- Entity Framework

- jQuery e jQuery Bootgrid

- HTML5, CSS3 e Bootstrap

## 📂 Estrutura do Projeto

``` bash
/DemoCrud
│
├── Controllers/        # Controladores MVC
├── Models/             # Modelos de dados
├── Views/              # Camada de visualização (Razor)
├── Infra/              # Lógica de infraestrutura
├── ViewModels/         # Modelos de visualização
└── Scripts/            # Scripts JS
```
## 📝 Como Executar

- 1️⃣ Clone o repositório:
``` bash
git clone https://github.com/seu-usuario/DemoCrud.git
```

- 2️⃣ Configure a string de conexão no Web.config:

``` bash
<connectionStrings>
    <add name="DefaultConnection" connectionString="Data Source=.;Initial Catalog=CatalogoLivros;Integrated Security=True" />
</connectionStrings>
```

- 3️⃣ Execute no Visual Studio (F5) e acesse:


- http://localhost:52125/Livros/Index

## 🖇️ Explicação dos using
``` bash
using DemoCrud.ViewModels;   // ViewModel para paginação
using System;                // Tipos básicos do .NET
using System.Collections.Generic; // Coleções genéricas
using System.Linq;           // Consultas LINQ
using System.Web;            // Funcionalidades web
using System.Web.Mvc;        // Recursos do ASP.NET MVC
```

## 🤝 Contribuição

- Faça um fork

- Crie uma nova branch:
``` bash
git checkout -b minha-feature
```
- Faça o commit das suas alterações:
``` bash
git commit -m 'Minha nova feature'
```
- Faça o push para a branch:
``` bash
git push origin minha-feature
```
Abra um Pull Request 📩
