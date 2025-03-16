import React, { useState, useEffect } from 'react';
import {
  Card,
  CardHeader,
  CardBody,
  Row,
  Col,
  Table,
  Button,
  Input,
  InputGroup,
  InputGroupText,
} from 'reactstrap';
import { useNavigate } from 'react-router-dom';
import { FaSearch, FaEye, FaPencilAlt } from 'react-icons/fa';
import api from '../../services/api';
import { formatDate } from '../../utils/format';
import { toast } from 'react-toastify';

const Products = () => {
  const navigate = useNavigate();
  const [products, setProducts] = useState([]);
  const [searchTerm, setSearchTerm] = useState('');
  const [loading, setLoading] = useState(false);
  const [page, setPage] = useState(1);
  const [totalPages, setTotalPages] = useState(0);

  const loadProducts = async () => {
    try {
      setLoading(true);
      const response = await api.get('/api/products', {
        params: {
          searchTerm,
          page,
          pageSize: 10,
        },
      });
      setProducts(response.data.items);
      setTotalPages(response.data.totalPages);
    } catch (error) {
      toast.error('Erro ao carregar produtos');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadProducts();
  }, [page, searchTerm]);

  const handleSearch = (event) => {
    setSearchTerm(event.target.value);
    setPage(1);
  };

  const handlePageChange = (newPage) => {
    if (newPage >= 1 && newPage <= totalPages) {
      setPage(newPage);
    }
  };

  return (
    <div className="content">
      <Row>
        <Col>
          <Card>
            <CardHeader>
              <Row className="align-items-center">
                <Col xs="6">
                  <h3 className="mb-0">Produtos</h3>
                </Col>
                <Col xs="6" className="text-end">
                  <Button
                    color="primary"
                    onClick={() => navigate('/admin/products/new')}
                  >
                    Novo Produto
                  </Button>
                </Col>
              </Row>
            </CardHeader>
            <CardBody>
              <Row className="mb-3">
                <Col>
                  <InputGroup>
                    <InputGroupText>
                      <FaSearch />
                    </InputGroupText>
                    <Input
                      placeholder="Buscar produtos..."
                      value={searchTerm}
                      onChange={handleSearch}
                    />
                  </InputGroup>
                </Col>
              </Row>
              <Table responsive>
                <thead>
                  <tr>
                    <th>Nome</th>
                    <th>Data de Criação</th>
                    <th>Ações</th>
                  </tr>
                </thead>
                <tbody>
                  {products.map((product) => (
                    <tr key={product.id}>
                      <td>{product.name}</td>
                      <td>{formatDate(product.createdAt)}</td>
                      <td>
                        <Button
                          color="info"
                          size="sm"
                          className="mr-2"
                          onClick={() => navigate(`/admin/products/${product.id}`)}
                        >
                          <FaEye />
                        </Button>
                        <Button
                          color="primary"
                          size="sm"
                          onClick={() =>
                            navigate(`/admin/products/${product.id}/edit`)
                          }
                        >
                          <FaPencilAlt />
                        </Button>
                      </td>
                    </tr>
                  ))}
                </tbody>
              </Table>
              {totalPages > 1 && (
                <Row className="justify-content-center">
                  <Col xs="auto">
                    <Button
                      color="primary"
                      onClick={() => handlePageChange(page - 1)}
                      disabled={page === 1}
                    >
                      Anterior
                    </Button>
                    <span className="mx-3">
                      Página {page} de {totalPages}
                    </span>
                    <Button
                      color="primary"
                      onClick={() => handlePageChange(page + 1)}
                      disabled={page === totalPages}
                    >
                      Próxima
                    </Button>
                  </Col>
                </Row>
              )}
            </CardBody>
          </Card>
        </Col>
      </Row>
    </div>
  );
};

export default Products; 