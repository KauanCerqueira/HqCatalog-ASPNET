# 📚 HqCatalog-ASPNET

> Aplicação ASP.NET MVC para gerenciamento de um catálogo de histórias em quadrinhos.

---

## 🚀 Tecnologias Utilizadas

- ASP.NET MVC
- Entity Framework Core
- HTML5
- CSS3
- JavaScript (ES6)

---

## 🧰 Funcionalidades

- [x] Cadastro de HQs com informações detalhadas.
- [x] Edição e remoção de títulos existentes.
- [x] Visualização de uma lista completa de HQs cadastradas.
- [x] Pesquisa de HQs por título, autor ou editora.

---

## 📂 Estrutura do Projeto

```
/HqCatalog-ASPNET
├── HqCatalog.Mvc           # Projeto principal ASP.NET MVC
├── HqCatalog.Business      # Lógica de negócios
├── HqCatalog.Data          # Acesso a dados e contexto do Entity Framework
├── HqCatalog.Api           # API para integração com outras plataformas
└── Database                # Scripts e backups do banco de dados
```

---

## 🔧 Como Executar o Projeto

1. **Clone o repositório:**

   ```bash
   git clone https://github.com/KauanCerqueira/HqCatalog-ASPNET.git
   ```

2. **Configure o banco de dados:**

   - Certifique-se de que o SQL Server está instalado e em execução.
   - Restaure o banco de dados utilizando os scripts da pasta `Database`.

3. **Atualize a string de conexão:**

   - No projeto `HqCatalog.Mvc`, localize o arquivo `appsettings.json`.
   - Atualize a propriedade `ConnectionStrings` com as credenciais do seu banco de dados.

4. **Execute a aplicação:**

   - Abra o arquivo `HqCatalog.sln` no Visual Studio.
   - Defina `HqCatalog.Mvc` como projeto de inicialização.
   - Pressione `F5` para compilar e executar a aplicação.

---

## 🛠️ Requisitos

- Visual Studio
- SQL Server
- .NET Framework

---

## 🤝 Contribuições

Contribuições são bem-vindas! Sinta-se à vontade para abrir issues e pull requests.

---

## 📄 Licença

Este projeto está sob a licença MIT. Veja o arquivo [LICENSE](LICENSE) para mais detalhes.

---

Feito com ❤️ por [Kauan Cerqueira](https://github.com/KauanCerqueira)

