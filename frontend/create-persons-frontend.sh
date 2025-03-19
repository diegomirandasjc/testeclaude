#!/bin/bash
# Script para criar as páginas frontend de gerenciamento de Pessoas

echo "Iniciando criação das páginas frontend para Pessoas..."

# Criar diretório para os componentes de Person se não existir
mkdir -p src/views/pages/persons

# Criando o arquivo de listagem de pessoas (Persons.js)
echo "Criando arquivo de listagem de pessoas (Persons.js)..."
cat > src/views/pages/persons/Persons.js << 'EOL'
import React, { useState, useEffect } from "react";
import {
  Card,
  CardHeader,
  CardBody,
  Container,
  Row,
  Col,
  Table,
  Button,
  Input,
  Alert,
  InputGroup,
  InputGroupAddon,
  InputGroupText,
  Pagination,
  PaginationItem,
  PaginationLink
} from "reactstrap";
import { useNavigate } from "react-router-dom";
import SimpleHeader from "components/Headers/SimpleHeader.js";
import api from "services/api";

const Persons = () => {
  const navigate = useNavigate();
  const [persons, setPersons] = useState([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);
  const [searchTerm, setSearchTerm] = useState("");
  const [pagination, setPagination] = useState({
    currentPage: 1,
    totalPages: 1,
    pageSize: 10
  });

  const loadPersons = async (search = "", page = 1) => {
    try {
      setLoading(true);
      setError(null);
      const response = await api.get("/api/persons", {
        params: {
          searchTerm: search,
          page,
          pageSize: pagination.pageSize
        }
      });

      if (!response.data || !response.data.items) {
        console.error('Invalid response format:', response.data);
        setError('Formato de resposta inválido');
        setPersons([]);
        return;
      }

      setPersons(response.data.items);
      setPagination(prev => ({
        ...prev,
        currentPage: response.data.page || 1,
        totalPages: response.data.totalPages || 1,
        totalItems: response.data.totalCount || 0
      }));
    } catch (err) {
      console.error("Erro ao carregar pessoas:", err);
      setError(err.message || "Erro ao carregar pessoas");
      setPersons([]);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    const delayDebounceFn = setTimeout(() => {
      loadPersons(searchTerm, 1);
    }, 500);

    return () => clearTimeout(delayDebounceFn);
  }, [searchTerm]);

  const handlePageChange = (page) => {
    loadPersons(searchTerm, page);
  };

  const renderPagination = () => {
    const pages = [];
    const maxVisiblePages = 5;
    let startPage = Math.max(1, pagination.currentPage - Math.floor(maxVisiblePages / 2));
    let endPage = Math.min(pagination.totalPages, startPage + maxVisiblePages - 1);

    if (endPage - startPage + 1 < maxVisiblePages) {
      startPage = Math.max(1, endPage - maxVisiblePages + 1);
    }

    // Botão "Anterior"
    pages.push(
      <PaginationItem key="prev" disabled={pagination.currentPage === 1}>
        <PaginationLink
          previous
          onClick={() => handlePageChange(pagination.currentPage - 1)}
        />
      </PaginationItem>
    );

    // Primeira página
    if (startPage > 1) {
      pages.push(
        <PaginationItem key={1}>
          <PaginationLink onClick={() => handlePageChange(1)}>1</PaginationLink>
        </PaginationItem>
      );
      if (startPage > 2) {
        pages.push(
          <PaginationItem key="ellipsis1" disabled>
            <PaginationLink>...</PaginationLink>
          </PaginationItem>
        );
      }
    }

    // Páginas visíveis
    for (let i = startPage; i <= endPage; i++) {
      pages.push(
        <PaginationItem key={i} active={i === pagination.currentPage}>
          <PaginationLink onClick={() => handlePageChange(i)}>{i}</PaginationLink>
        </PaginationItem>
      );
    }

    // Última página
    if (endPage < pagination.totalPages) {
      if (endPage < pagination.totalPages - 1) {
        pages.push(
          <PaginationItem key="ellipsis2" disabled>
            <PaginationLink>...</PaginationLink>
          </PaginationItem>
        );
      }
      pages.push(
        <PaginationItem key={pagination.totalPages}>
          <PaginationLink onClick={() => handlePageChange(pagination.totalPages)}>
            {pagination.totalPages}
          </PaginationLink>
        </PaginationItem>
      );
    }

    // Botão "Próximo"
    pages.push(
      <PaginationItem key="next" disabled={pagination.currentPage === pagination.totalPages}>
        <PaginationLink
          next
          onClick={() => handlePageChange(pagination.currentPage + 1)}
        />
      </PaginationItem>
    );

    return pages;
  };

  return (
    <>
      <SimpleHeader name="Pessoas" parentName="Gestão" />
      <Container className="mt--6" fluid>
        <Row>
          <Col>
            <Card>
              <CardHeader>
                <Row className="align-items-center">
                  <Col xs="8">
                    <h3 className="mb-0">Lista de Pessoas</h3>
                  </Col>
                  <Col className="text-right" xs="4">
                    <Button
                      color="primary"
                      onClick={() => navigate("/admin/persons/new")}
                    >
                      Nova Pessoa
                    </Button>
                  </Col>
                </Row>
              </CardHeader>
              <CardBody>
                {error && (
                  <Alert color="danger" toggle={() => setError(null)}>
                    {error}
                  </Alert>
                )}

                <Row className="mb-4">
                  <Col md="4">
                    <InputGroup>
                      <InputGroupAddon addonType="prepend">
                        <InputGroupText>
                          <i className="fas fa-search" />
                        </InputGroupText>
                      </InputGroupAddon>
                      <Input
                        placeholder="Buscar pessoas..."
                        type="text"
                        value={searchTerm}
                        onChange={(e) => setSearchTerm(e.target.value)}
                      />
                    </InputGroup>
                  </Col>
                </Row>

                <Table className="align-items-center table-flush" responsive>
                  <thead className="thead-light">
                    <tr>
                      <th scope="col">Nome</th>
                      <th scope="col">Email</th>
                      <th scope="col">Data de Cadastro</th>
                      <th scope="col">Ações</th>
                    </tr>
                  </thead>
                  <tbody>
                    {loading ? (
                      <tr>
                        <td colSpan="4" className="text-center">
                          Carregando...
                        </td>
                      </tr>
                    ) : persons.length > 0 ? (
                      persons.map((person) => (
                        <tr key={person.id}>
                          <td>{person.name}</td>
                          <td>{person.email}</td>
                          <td>
                            {new Date(person.createdAt).toLocaleDateString('pt-BR', {
                              day: '2-digit',
                              month: '2-digit',
                              year: 'numeric',
                              hour: '2-digit',
                              minute: '2-digit'
                            })}
                          </td>
                          <td>
                            <Button
                              color="info"
                              size="sm"
                              className="mr-2"
                              onClick={() => navigate(`/admin/persons/${person.id}`)}
                            >
                              <i className="fas fa-eye" />
                            </Button>
                            <Button
                              color="primary"
                              size="sm"
                              onClick={() => navigate(`/admin/persons/${person.id}/edit`)}
                            >
                              <i className="fas fa-edit" />
                            </Button>
                          </td>
                        </tr>
                      ))
                    ) : (
                      <tr>
                        <td colSpan="4" className="text-center">
                          Nenhuma pessoa encontrada.
                        </td>
                      </tr>
                    )}
                  </tbody>
                </Table>

                {pagination.totalPages > 1 && (
                  <div className="d-flex justify-content-center mt-4">
                    <Pagination>
                      {renderPagination()}
                    </Pagination>
                  </div>
                )}
              </CardBody>
            </Card>
          </Col>
        </Row>
      </Container>
    </>
  );
};

export default Persons;
EOL

# Criando o arquivo de formulário de pessoa (PersonForm.js)
echo "Criando arquivo de formulário de pessoa (PersonForm.js)..."
cat > src/views/pages/persons/PersonForm.js << 'EOL'
import React, { useState, useEffect } from "react";
import {
  Card,
  CardHeader,
  CardBody,
  Container,
  Row,
  Col,
  Button,
  Form,
  FormGroup,
  Input,
  Label,
  Alert
} from "reactstrap";
import { useNavigate, useParams, useLocation } from "react-router-dom";
import SimpleHeader from "components/Headers/SimpleHeader.js";
import api from "services/api";

const PersonForm = () => {
  const navigate = useNavigate();
  const { id } = useParams();
  const location = useLocation();
  const isEditing = !!id;
  const isViewMode = location.pathname.split('/').pop() !== 'edit';

  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);
  const [formData, setFormData] = useState({
    name: "",
    email: ""
  });
  const [formErrors, setFormErrors] = useState({});

  const loadPerson = async () => {
    if (!id) return;

    try {
      setLoading(true);
      const response = await api.get(`/api/persons/${id}`);
      setFormData({
        name: response.data.name,
        email: response.data.email
      });
    } catch (err) {
      console.error("Erro ao carregar pessoa:", err);
      setError("Erro ao carregar dados da pessoa");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    if (isEditing) {
      loadPerson();
    }
  }, [id]);

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError(null);
    setFormErrors({});
    setLoading(true);

    try {
      if (isEditing) {
        await api.put(`/api/persons/${id}`, formData);
      } else {
        await api.post("/api/persons", formData);
      }
      navigate("/admin/persons");
    } catch (err) {
      console.error("Erro ao salvar pessoa:", err);
      if (err.response?.data) {
        setFormErrors(err.response.data);
      } else {
        setError("Erro ao salvar pessoa");
      }
    } finally {
      setLoading(false);
    }
  };

  const handleInputChange = (e) => {
    const { name, value } = e.target;
    setFormData(prev => ({
      ...prev,
      [name]: value
    }));
    if (formErrors[name]) {
      setFormErrors(prev => ({
        ...prev,
        [name]: null
      }));
    }
  };

  if (loading && isEditing) {
    return (
      <>
        <SimpleHeader name="Pessoa" parentName="Gestão" />
        <Container className="mt--6" fluid>
          <Card>
            <CardBody className="text-center">
              Carregando...
            </CardBody>
          </Card>
        </Container>
      </>
    );
  }

  return (
    <>
      <SimpleHeader name="Pessoa" parentName="Gestão" />
      <Container className="mt--6" fluid>
        <Row>
          <Col>
            <Card>
              <CardHeader>
                <h3 className="mb-0">
                  {isViewMode ? "Visualizar Pessoa" : (isEditing ? "Editar Pessoa" : "Nova Pessoa")}
                </h3>
              </CardHeader>
              <CardBody>
                {error && (
                  <Alert color="danger" toggle={() => setError(null)}>
                    {error}
                  </Alert>
                )}

                <Form onSubmit={handleSubmit}>
                  <Row>
                    <Col md="6">
                      <FormGroup>
                        <Label for="name">Nome</Label>
                        <Input
                          id="name"
                          name="name"
                          value={formData.name}
                          onChange={handleInputChange}
                          invalid={!!formErrors.Name}
                          disabled={isViewMode}
                        />
                        {formErrors.Name && (
                          <div className="invalid-feedback d-block">
                            {formErrors.Name.join(", ")}
                          </div>
                        )}
                      </FormGroup>
                    </Col>
                    <Col md="6">
                      <FormGroup>
                        <Label for="email">Email</Label>
                        <Input
                          id="email"
                          name="email"
                          type="email"
                          value={formData.email}
                          onChange={handleInputChange}
                          invalid={!!formErrors.Email}
                          disabled={isViewMode}
                        />
                        {formErrors.Email && (
                          <div className="invalid-feedback d-block">
                            {formErrors.Email.join(", ")}
                          </div>
                        )}
                      </FormGroup>
                    </Col>
                  </Row>
                  <Button color="secondary" onClick={() => navigate("/admin/persons")} className="mr-2">
                    Voltar
                  </Button>
                  {!isViewMode && (
                    <Button color="primary" type="submit" disabled={loading}>
                      {loading ? "Salvando..." : "Salvar"}
                    </Button>
                  )}
                </Form>
              </CardBody>
            </Card>
          </Col>
        </Row>
      </Container>
    </>
  );
};

export default PersonForm;
EOL

echo "Criação dos arquivos frontend concluída com sucesso!"
echo "Agora será necessário atualizar o arquivo de rotas para incluir as rotas para as novas páginas."
echo ""
echo "IMPORTANTE: Você precisa editar o arquivo de rotas manualmente. Por favor, adicione as seguintes rotas:"
echo ""
echo "// Rotas para Pessoas"
echo "{"
echo "  path: \"/admin/persons\","
echo "  element: <Persons />,"
echo "  key: \"persons\","
echo "},"
echo "{"
echo "  path: \"/admin/persons/new\","
echo "  element: <PersonForm />,"
echo "  key: \"new-person\","
echo "},"
echo "{"
echo "  path: \"/admin/persons/:id\","
echo "  element: <PersonForm />,"
echo "  key: \"view-person\","
echo "},"
echo "{"
echo "  path: \"/admin/persons/:id/edit\","
echo "  element: <PersonForm />,"
echo "  key: \"edit-person\","
echo "},"
echo ""
echo "E adicione os seguintes imports no topo do arquivo de rotas:"
echo ""
echo "import Persons from \"views/pages/persons/Persons\";"
echo "import PersonForm from \"views/pages/persons/PersonForm\";"
echo ""
echo "Além disso, você pode precisar atualizar o menu lateral para incluir um link para a página de listagem de pessoas."
