import React, { useState, useEffect } from 'react';
import {
  Card,
  CardHeader,
  CardBody,
  Row,
  Col,
  Form,
  FormGroup,
  Label,
  Input,
  Button,
} from 'reactstrap';
import { useNavigate, useParams } from 'react-router-dom';
import api from '../../services/api';
import { toast } from 'react-toastify';

const ProductForm = () => {
  const navigate = useNavigate();
  const { id } = useParams();
  const isView = window.location.pathname.endsWith('/view');
  const isEdit = window.location.pathname.endsWith('/edit');

  const [formData, setFormData] = useState({
    name: '',
  });

  const [formErrors, setFormErrors] = useState({});
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    if (id) {
      loadProduct();
    }
  }, [id]);

  const loadProduct = async () => {
    try {
      setLoading(true);
      const response = await api.get(`/api/products/${id}`);
      setFormData({
        name: response.data.name,
      });
    } catch (error) {
      toast.error('Erro ao carregar produto');
      navigate('/admin/products');
    } finally {
      setLoading(false);
    }
  };

  const handleSubmit = async (event) => {
    event.preventDefault();
    setFormErrors({});

    try {
      setLoading(true);
      if (isEdit) {
        await api.put(`/api/products/${id}`, formData);
        toast.success('Produto atualizado com sucesso');
      } else {
        await api.post('/api/products', formData);
        toast.success('Produto criado com sucesso');
      }
      navigate('/admin/products');
    } catch (error) {
      if (error.response?.data) {
        setFormErrors(error.response.data);
      }
      toast.error(
        error.response?.data?.message || 'Erro ao salvar produto'
      );
    } finally {
      setLoading(false);
    }
  };

  const handleChange = (event) => {
    const { name, value } = event.target;
    setFormData((prev) => ({ ...prev, [name]: value }));
  };

  return (
    <div className="content">
      <Row>
        <Col>
          <Card>
            <CardHeader>
              <Row className="align-items-center">
                <Col xs="6">
                  <h3 className="mb-0">
                    {isView
                      ? 'Visualizar Produto'
                      : isEdit
                      ? 'Editar Produto'
                      : 'Novo Produto'}
                  </h3>
                </Col>
              </Row>
            </CardHeader>
            <CardBody>
              <Form onSubmit={handleSubmit}>
                <FormGroup>
                  <Label>Nome</Label>
                  <Input
                    name="name"
                    value={formData.name}
                    onChange={handleChange}
                    invalid={!!formErrors.Name}
                    disabled={isView || loading}
                  />
                  {formErrors.Name && (
                    <div className="invalid-feedback d-block">
                      {formErrors.Name}
                    </div>
                  )}
                </FormGroup>
                {!isView && (
                  <Button
                    type="submit"
                    color="primary"
                    disabled={loading}
                  >
                    {loading ? 'Salvando...' : 'Salvar'}
                  </Button>
                )}
                <Button
                  type="button"
                  color="secondary"
                  className="ml-2"
                  onClick={() => navigate('/admin/products')}
                  disabled={loading}
                >
                  Voltar
                </Button>
              </Form>
            </CardBody>
          </Card>
        </Col>
      </Row>
    </div>
  );
};

export default ProductForm; 